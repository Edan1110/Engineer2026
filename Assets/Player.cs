using UnityEngine;
using System.Collections;



public class Player : MonoBehaviour
{
    

    float mvSpeed = Config.fdata[0], gxzero = Config.fdata[1],
    gxcoef = Config.fdata[2], gyzero = Config.fdata[3], 
    gycoef  = Config.fdata[4], lcoef = Config.fdata[5], 
    scoef = Config.fdata[6], reloadCooldown = Config.fdata[7],
    volCoef = Config.fdata[8], delta, mag, d = 0, ll, toLoad = Config.fdata[10], toShoot = Config.fdata[11];
    int cur = -1, temp, speed = Animator.StringToHash("Speed"), 
    animSpeed = Animator.StringToHash("animSpeed"), 
    draw = Animator.StringToHash("draw"), duration = Config.idata[0];
    float[] vals;
    bool movable = true, paused = false;
    Rigidbody rb;
    Vector3 tl;

    SC rx = new SC(0, 0.9f, 4, 0.1f), ry = new SC(0, 0.9f, 4, 0.1f);
    public Transform CB;
    Bow bow;
    Animator cameraAnim, CBAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vals = new float[duration];
        P.camera = CB.GetChild(1);
        rb = GetComponent<Rigidbody>();
        bow = CB.GetChild(0).GetComponent<Bow>();
        cameraAnim = P.camera.GetComponent<Animator>();
        CBAnim = CB.GetComponent<Animator>();
    }
    

    // Update is called once per frame
    void Update()
    {
        //Initial Conf
        if(!P.toStart) return;
        //Loadcell Values
        if(++cur == duration) cur = 0;
        vals[cur] = P.fdata[6];

        // Debug.Log(vals[cur]);

        //Movement
        tl  = Vector3.zero;
        tl = (P.idata[0]-512)/512f * transform.right + (512-P.idata[1])/512f * transform.forward;
        // Debug.Log(P.idata[0]+" "+P.idata[1]);
        mag = tl.magnitude;
        if (movable && mag>0.2f)
        {
            if(bow.isNull())
            {
                rb.linearVelocity= tl * mvSpeed;
                cameraAnim.SetFloat(speed, mag);
                cameraAnim.SetFloat(animSpeed, mag*0.45f + 0.55f);
            }
            else
            {
                rb.linearVelocity = tl * mvSpeed * 0.6f;
                cameraAnim.SetFloat(speed, mag * 0.4f);
                cameraAnim.SetFloat(animSpeed, mag*0.4f + 0.4f);
            }
        }
        else
        {   
            cameraAnim.SetFloat(speed, 0f);
        }

        //Rotation
        if(!paused){
            rx.update(P.fdata[0] - gxzero);
            ry.update(P.fdata[2] - gyzero);
            transform.eulerAngles = Vector3.up *ry.value * gycoef;
            CB.localEulerAngles = Vector3.right * rx.value * gxcoef;
        }

        if (bow.isNull())
        {
            d += Time.deltaTime;

            if(vals[cur]>toLoad && d > reloadCooldown)
            {
                P.auso[0].Play();
                StartCoroutine(bow.Load(Random.value < 0.5f));
                cameraAnim.speed = 0f;
            }   
        }
        else{
            ll = vals[cur] * lcoef;
            // Debug.Log(vals[cur] - vals[(cur == duration-1) ? 0 : cur+1]);
            //Shoot
            if((delta = vals[(cur == duration-1) ? 0 : cur+1] - vals[cur]) > toShoot || P.stickbtn){
                StartCoroutine(pause());
                cameraAnim.speed = 1f;
                bow.Shoot(scoef * delta);
                ll = 0f;
                Debug.Log(delta * scoef);
            
                CBAnim.SetFloat(draw, 0);
                StartCoroutine(Knockback(delta));
                temp = Random.Range(3, 5);
                P.auso[temp].volume = volCoef * vals[cur];
                P.auso[temp].Play();
                P.vib = 0;
                d = 0f;
            }
            //Unload
            else if (vals[cur] < toLoad)
            {
                cameraAnim.speed = 1f;
                d = 0f;
                P.vib = 0;
                ll = 0f;
                CBAnim.SetFloat(draw, 0);
                bow.Unload();
            }
            //Set
            else
            {
                bow.setLoad(ll);
                CBAnim.SetFloat(draw, ll);
                P.vib = (byte)(ll * 256);
            }

        }
    }

    IEnumerator Knockback(float f)
    {
        movable = false;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(-f * P.camera.forward);
        yield return new WaitForSeconds(reloadCooldown);
        movable = true;
    }
    IEnumerator pause()
    {
        paused = true;
        yield return new WaitForSeconds(.8f);
        paused = false;
    }

}
