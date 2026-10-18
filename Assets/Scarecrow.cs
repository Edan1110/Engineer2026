using UnityEngine;


public class Scarecrow : MonoBehaviour, Target
{
    bool trigger = false;
    float t = 0;
    public void onHit(ContactPoint contact,Arrow arrow)
    {
        arrow.rb.linearVelocity = Vector3.zero;
        arrow.rb.angularVelocity = Vector3.zero;
        // arrow.rb.isKinematic = true;
        P.auso.PlayOneShot(P.audios[2], 100 / Vector3.SqrMagnitude(P.player.transform.position - contact.point));
        StartCoroutine(P.player.Get3());

    }
    void Update()
    {
        if (trigger)
        {
            t += Time.deltaTime;
            transform.position = new Vector3(5 - t * 2, 0.1f, -26);
            if(t>9) {trigger = false; Destroy(gameObject);}
        }   
    }

    public void emerge()
    {
        // if(trig)
        t = 0;
        transform.rotation = Quaternion.Euler(0, 130, 0);
        transform.position = new Vector3(5f, 0.1f, -26);
        trigger = true;
        
    }
}
