using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;



public class Player : MonoBehaviour
{

    public float mvSpeed, gxzero, gxcoef, gyzero, gycoef, lcoef, scoef, reloadCooldown;
    int lzero, l, cur = -1, prev, 
    speed = Animator.StringToHash("Speed"), 
    animSpeed = Animator.StringToHash("animSpeed"), 
    draw = Animator.StringToHash("draw");
    int[] vals;
    public int duration, toLoad, toShoot;
    float mag, d = 0, ll;
    bool movable = true, lset = false;
    Rigidbody rb;
    Vector3 tl;

    SC rx = new SC(0, 0.9f, 4, 0.1f), ry = new SC(0, 0.9f, 4, 0.1f);
    public Transform CB;
    Bow bow;
    Animator cameraAnim, CBAnim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vals = new int[duration];
        P.camera = CB.GetChild(1);
        rb = GetComponent<Rigidbody>();
        bow = CB.GetChild(0).GetComponent<Bow>();
        cameraAnim = P.camera.GetComponent<Animator>();
        CBAnim = CB.GetComponent<Animator>();

        // t.text = "HH";
    }
    

    // Update is called once per frame
    void Update()
    {
        //Initial Conf
        if(!P.toStart) return;
        if (!lset)
        {
            lzero = P.idata[0];
            lset = true;
        }
        
        //Loadcell Values
        if(++cur == duration) cur = 0;
        l = lzero - (vals[cur] = P.idata[0]);
        // string s = "";
        
        // t.text = s;
        //Movement
        tl = (P.idata[1]-512)/512f * transform.right + (512-P.idata[2])/512f * transform.forward;
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
        rx.update(P.fdata[0] - gxzero);
        ry.update(P.fdata[2] - gyzero);
        transform.eulerAngles = Vector3.up *ry.value * gycoef;
        CB.localEulerAngles = Vector3.right * rx.value * gxcoef;

        if (bow.isNull())
        {
            d += Time.deltaTime;
            if(l>toLoad && d > reloadCooldown)
            {
                StartCoroutine(bow.Load(Random.value < 0.5f));
                cameraAnim.speed = 0f;
            }   
        }
        else{
            ll = l * lcoef;
            //Shoot
            if(vals[cur] - (prev = vals[(cur == duration-1) ? 0 : cur+1]) > toShoot || P.stickbtn){
                cameraAnim.speed = 1f;
                bow.Shoot(scoef);
                ll = 0f;
                CBAnim.SetFloat(draw, 0);
                StartCoroutine(Knockback(l * scoef));
                // Debug.Log(l * scoef);
                // Knockback(100000);
                Debug.Log(lzero - prev);
                P.vib = 0;
                d = 0f;
            }
            //Unload
            else if (l < toLoad)
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

}
