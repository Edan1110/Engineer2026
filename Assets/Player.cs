using UnityEngine;
using TMPro;
using System.Data;
using NUnit.Framework.Constraints;

public class Player : MonoBehaviour
{
    public float mvSpeed, gxzero, gxcoef, gyzero, gycoef, lcoef, scoef, reloadCooldown;
    float x, y, d = 0;
    // public float 
    int[] vals;
    public int duration, toLoad, toShoot;
    int lzero, l, cur = -1, prev;
    Arrow curArrow = null;

    Rigidbody rb;

    public GameObject Arrow;
    bool isMoving = false, knockback = false, lset = false;

    int knockbackCount = 0;
    Vector3 startVel, targetVel;

    Transform camera, bow;

    public TMP_Text text;
        SC rx, ry;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // rb.maxLinearVelocity = mvSpeed;
        camera = transform.GetChild(0);
        bow = camera.GetChild(0);
        vals = new int[duration];
        rx = new SC(0, 0.9f, 4, 0.1f);
        ry = new SC(0, 0.9f, 4, 0.1f);
        // delta = 0;
        
        
        // cameraAnim = camera.GetComponent<Animator>();
        // shaker = pivot.GetComponent<CameraShake>();
    }
    

    // Update is called once per frame
    void Update()
    {
        if(!P.toStart) return;
        if (!lset)
        {
            lzero = P.idata[0];
            lset = true;
        }
        // Debug.Log("Hello");

        if(++cur == duration) cur = 0;
        l = lzero - (vals[cur] = P.idata[0]);

        // string s = "";
        // for(int i = 0; i<duration; i++)
        // {
        //     s+=vals[i].ToString() + ", ";
        // }

        // Debug.Log(P.idata[0]);
        

        // P.vib = 122; 

        rb.linearVelocity
        = ((452 < P.idata[1] && P.idata[1] < 572) ? 0 : (P.idata[1] - 512) * mvSpeed) * transform.right +
        ((452 < P.idata[2] && P.idata[2] < 572) ? 0 : (512-P.idata[2]) * mvSpeed) * transform.forward;
        // accel
        // transform.Translate(x, 0, y);
        // Vector3.Dot(rb.linearVelocity, )
        // Vector3.Project(rb.linearVelocity, p)

        // if (knockback)
        // {
        //     if(knockbackCount == 0){
        //         targetVel = transform.right * x + transform.forward * y;
        //         startVel = rb.linearVelocity;
        //     }
        //     else
        //     {
        //         rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVel, knockbackCount/100f);
        //         if(knockbackCount == 100)
        //         {
        //             knockback = false;
        //             knockbackCount = 0;
        //         }
        //     }
        //     knockbackCount++;
        // }
        // else{
        //     rb.linearVelocity = transform.right * x + transform.forward * y;
            // Debug.Log(P.fdata[0]);
        // }
        rx.update(P.fdata[0] - gxzero);
        ry.update(P.fdata[2] - gyzero);
        
        transform.eulerAngles = Vector3.up *ry.value * gycoef;
        camera.localEulerAngles = Vector3.right * rx.value * gxcoef;
 
        if(curArrow == null){
            d += Time.deltaTime;
            if(l>toLoad && d > reloadCooldown)
            {
                Draw();
            }
        }
        else{
            curArrow.transform.localPosition = new Vector3(l*lcoef, -1, 0);
            
            // Debug.Log(vals[cur] - vals[prev = (cur == duration-1) ? 0 : cur+1]);
            if(vals[cur] - (prev = vals[(cur == duration-1) ? 0 : cur+1]) > toShoot){
                d = 0;
                Debug.Log(lzero - prev);
                // Destroy(curArrow.gameObject);
                curArrow.Shoot((lzero-prev) * scoef, duration);
                curArrow.transform.SetParent(null, true);
                curArrow = null;
            }
        }

    text.text = $"{l}\n{P.idata[2]}, {P.idata[1]}";
        
    }

    void Draw()
    {
        curArrow = Instantiate(Arrow).GetComponent<Arrow>();
        curArrow.transform.SetParent(bow, true);
        curArrow.transform.localPosition = new Vector3(0, -1, 0);
        // curArrow.transform.position = camera.position + camera.forward * 0.7f + camera.right * 0.2f;

    }
    float cutoff(float v, float cut){ return (v < cut && v > -cut) ? 0f : v;}
}
