using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;




public class Player : MonoBehaviour
{
    

    Volume v;
    public AudioClip[] auds; 
    public RawImage img;
    public TMP_Text timer, sctxt;
    Animator scanim;
    Color c;
    readonly float mvSpeed = Config.fdata[0], gxzero = Config.fdata[1],
    gxcoef = Config.fdata[2], gyzero = Config.fdata[3], 
    gycoef  = Config.fdata[4], lcoef = Config.fdata[5], 
    scoef = Config.fdata[6], reloadCooldown = Config.fdata[7],
    volCoef = Config.fdata[8],  toLoad = Config.fdata[10], toShoot = Config.fdata[11];
    float delta, mag, d = 0, ll;
    readonly int speed = Animator.StringToHash("Speed"), 
    animSpeed = Animator.StringToHash("animSpeed"), 
    idleSpeed = Animator.StringToHash("idleSpeed"), get = Animator.StringToHash("Get"),
    draw = Animator.StringToHash("draw"), duration = Config.idata[0];

    int cur = -1;
    public static int score;
    float[] vals;
    bool movable = true, paused = false;
    Rigidbody rb;
    Vector3 tl;

    SC rx = new SC(0, 0.9f, 4, 0.1f), ry = new SC(0, 0.9f, 4, 0.1f);
    public Transform CB;
    Bow bow;
    Animator cameraAnim, CBAnim;

    public void Get1()
    {
        scanim.SetTrigger(get);
        score += 1;
        sctxt.text = score.ToString();
    }
    public IEnumerator Get3()
    {
        scanim.SetTrigger(get);
    
        int start = score;
        score += 3;
    
        for (int i = start; i <= score; i++)
        {
            sctxt.text = i.ToString();
            yield return new WaitForSeconds(2f / 7f);
        }
    }
    IEnumerator setTimer()
    {
        yield return new WaitUntil(()=>P.toStart);
        for(int i = Config.idata[1]; i>=0; i--){
            if(i == 10){
                timer.color = Color.red;
                P.auso.PlayOneShot(P.audios[6]);
            }
            timer.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        SceneManager.LoadScene("End");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        score = 0;
        P.toStart = false;
        StartCoroutine(setTimer());
        c = img.color;
        v = GetComponent<Volume>();
        scanim = sctxt.GetComponent<Animator>();
        P.audios = auds;
        P.auso = GetComponent<AudioSource>();
        vals = new float[duration];
        P.camera = CB.GetChild(1).GetComponent<Camera>();
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
                P.auso.PlayOneShot(auds[0]);
                StartCoroutine(bow.Load(Random.value < 0.5f));
                cameraAnim.SetFloat(idleSpeed, 0f);
                // cameraAnim.speed = 0f;
                StartCoroutine(campause());
            }   
        }
        else{
            ll = vals[cur] * lcoef;
            // Debug.Log(vals[cur] - vals[(cur == duration-1) ? 0 : cur+1]);
            //Shoot
            if((delta = vals[(cur == duration-1) ? 0 : cur+1] - vals[cur]) > toShoot || P.stickbtn){
                cameraAnim.SetFloat(idleSpeed, 1f);
                bow.Shoot(scoef * delta);
                ll = 0f;
                Debug.Log(delta * scoef);
            
                CBAnim.SetFloat(draw, 0);
                // StartCoroutine(Knockback(delta));
                P.auso.PlayOneShot(auds[Random.Range(3, 6)], volCoef * delta);
                P.vib = 0;
                d = 0f;
                
                StartCoroutine(camunpause(true));
            }
            //Unload
            else if (vals[cur] < toLoad)
            {
                cameraAnim.SetFloat(idleSpeed, 1f);
                d = 0f;
                P.vib = 0;
                ll = 0f;
                CBAnim.SetFloat(draw, 0);
                bow.Unload();
                StartCoroutine(camunpause(false));
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
        rb.AddForce(-f * P.camera.transform.forward);
        yield return new WaitForSeconds(reloadCooldown);
        movable = true;
    }
    IEnumerator campause()
    {
        rx.stop();
        ry.stop();
        paused = true;
        for(int i = 1; i<21; i++)
        {
            if(i<16){
                v.weight = c.a = i/16f;
                img.color = c;
            }
            P.camera.fieldOfView = 60 - i/2f;
            yield return null;
        }
    }
    IEnumerator camunpause(bool wait)
    {
        if(wait) yield return new WaitForSeconds(0.7f);
        paused = false;
        for(int i = 1; i<21; i++)
        {
            if(i>4){
                v.weight = c.a = (20-i)/15f;
                img.color = c;
            }

            P.camera.fieldOfView = 40 + i/2f;
            yield return null;
        }
    }
}
