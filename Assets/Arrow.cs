using UnityEngine;
using System.Collections;

enum ArrowState {load, shooting, shot, stuck, none, unload}
public class Arrow : MonoBehaviour
{
    Rigidbody rb;
    
    ArrowState state;
    Transform tail, tip;
    Collider tipCollider;
    
    Vector3 pointVelocity;
    public float lateralDrag;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Debug.Log("rb");
        state = ArrowState.load;
        tail = transform.GetChild(0);
        tip = transform.GetChild(1);
        tipCollider = tip.GetComponent<Collider>();
        rb.detectCollisions = false;
        // Shoot(1000f, 10);
    }
    IEnumerator ApplyForceForFrames(Rigidbody rb, Vector3 force, int k)
    {
        rb.detectCollisions = true;
    rb.useGravity = true;
    state = ArrowState.shooting;
    for (int i = 0; i < k; i++)
    {
        rb.AddForce(force, ForceMode.Force);
        yield return new WaitForFixedUpdate(); // 물리 프레임 대기
    }
    state = ArrowState.shot;
}

    public void Unload()
    {
        state = ArrowState.unload;
        rb.isKinematic = false;
        rb.detectCollisions = true;
        rb.AddForce(Vector3.up * 50f);
    }
    public void Shoot(float f, int k)
    {
        StartCoroutine(ApplyForceForFrames(rb, transform.forward * f, k));
        // StartCoroutine(ApplyForceForFrames(playerrb, transform.forward * -f * rb.mass / playerrb.mass, k));
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == ArrowState.shooting || state == ArrowState.shot)
        {
            pointVelocity = rb.GetPointVelocity(tail.position);
            Vector3 lateralVelocity = pointVelocity - Vector3.Project(pointVelocity, transform.forward);
            // Debug.Log(lateralDrag);
            // Debug.Log(lateralVelocity);
            rb.AddForceAtPosition(-lateralDrag * lateralVelocity.magnitude * lateralVelocity, tail.position);
        }      
    }

    void OnCollisionEnter(Collision collision)
    {
        if(state == ArrowState.load || state == ArrowState.stuck) return;

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

                // 화살 방향 정렬
        // transform.rotation = Quaternion.LookRotation(-tipContact.normal, transform.up);

        // 회전 후 화살촉 끝점을 접촉 위치로 이동
        transform.position += tipContact.point - tip.position;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.detectCollisions = false;

        // transform.SetParent(collision.transform, true);
        // collision.transform.GetComponent<Target>().onHit(gameObject);        
    }
    }
}
