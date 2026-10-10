using UnityEngine;


public class Scarecrow : MonoBehaviour, Target
{
    Transform piv;
    Vector3 pos, vec = Vector3.zero, rotvec;
    float ang = 0f;
    int hit = Animator.StringToHash("hit");
    Animator anim;
    public float angper = 0f;
    public bool start = false;
    void Start()
    {
        piv = transform.GetChild(0);
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if(!start) return;
        pos = piv.position;
        transform.rotation = Quaternion.AngleAxis(ang * angper, rotvec);
        transform.position += pos - piv.position;
    }
    public void onHit(ContactPoint contact,Arrow arrow)
    {
        vec = Vector3.ProjectOnPlane(arrow.rb.linearVelocity, piv.up);
        ang = 100 * vec.sqrMagnitude;
        anim.SetTrigger(hit);
        rotvec = Vector3.Cross(piv.up, vec);

        arrow.rb.linearVelocity = Vector3.zero;
        arrow.rb.angularVelocity = Vector3.zero;
        arrow.rb.isKinematic = true;
        P.auso[2].volume = 100 / Vector3.SqrMagnitude(P.player.position - contact.point);
        P.auso[2].Play();
    }
}
