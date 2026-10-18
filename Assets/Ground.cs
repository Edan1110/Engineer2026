using UnityEngine;
using System.Collections;


public class Ground : MonoBehaviour, Target
{
    public static IEnumerator destroyAfter(float f, Object g)
    {
        yield return new WaitForSeconds(f);
        if(g != null) Destroy(g);
    }
    public void onHit(ContactPoint contact,Arrow arrow)
    {
        arrow.rb.linearVelocity = Vector3.zero;
        arrow.rb.angularVelocity = Vector3.zero;
        arrow.rb.isKinematic = true;
        // P.auso.PlayOneShot(P.audios[2], 10 / Vector3.SqrMagnitude(P.player.position - contact.point));
        StartCoroutine(destroyAfter(5, arrow.gameObject));
    }
}
