using UnityEngine;
using System.Collections;
using System.Drawing.Drawing2D;

public enum ArrowState {load, shot, hit, none, unload, loading}
public class Arrow : MonoBehaviour
{
    public Rigidbody rb;
    public ArrowState state;
    Transform tail, tip;
    Collider tipCollider;
    Target t;
    
    Vector3 tailVelocity;
    public float tailDrag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Debug.Log("rb");
        state = ArrowState.loading;
        tail = transform.GetChild(0);
        tip = transform.GetChild(1);
        tipCollider = tip.GetComponent<Collider>();
        rb.detectCollisions = false;
    }


    public void Unload()
    {
        // rb.interpolation = RigidbodyInterpolation.Interpolate;
        state = ArrowState.unload;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.AddForce(Vector3.up * 50f);
    }

    public void Shoot(float f)
    {
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.useGravity = true;
        rb.AddForce(transform.forward * f, ForceMode.Force);
        state = ArrowState.shot;
    }

    void FixedUpdate()
    {
        if (state == ArrowState.shot){
            Vector3 temp = rb.GetPointVelocity(tail.position);
            tailVelocity = temp - Vector3.Project(temp, transform.forward);
            rb.AddForceAtPosition(-tailDrag * tailVelocity.magnitude * tailVelocity, tail.position);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if(state == ArrowState.load || state == ArrowState.hit) return;
        if(collision.gameObject.CompareTag("Target")){
            bool tipHit = false;
            ContactPoint tipContact = default;

            foreach(ContactPoint contact in collision.contacts)
            {
                if(contact.thisCollider == tipCollider)
                {
                    tipContact = contact;
                    tipHit = true;
                    break;
                }
            }
            if(!tipHit) return;   

            transform.position += tipContact.point - tip.position;

            rb.detectCollisions = false;
            t = collision.gameObject.GetComponent<Target>();
            if (t == null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            else t.onHit(
                tipContact.point,       // 맞은 월드 좌표
                tipContact.normal,      // 표면 법선
                rb.linearVelocity,      // 충돌 순간 속도
                rb.angularVelocity,     // 충돌 순간 각속도
                this                    // 맞힌 화살
            );
            state = ArrowState.hit;
  
        }
    }
}
