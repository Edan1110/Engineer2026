using System.Collections;
using UnityEngine;

public class Archery : MonoBehaviour, Target
{
    Transform stand, piv;
    Vector3 pivpos;
    public bool enb = false;
    public void onHit(ContactPoint contact,Arrow arrow)
    {
        arrow.rb.linearVelocity = Vector3.zero;
        arrow.rb.angularVelocity = Vector3.zero;
        // arrow.rb.isKinematic = true;
        Ground.destroyAfter(3f, arrow.gameObject);
        if (enb)
        {
            P.auso.PlayOneShot(P.audios[1]);
            P.player.Get1();
            StartCoroutine(disable());
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stand = transform.parent.GetChild(0);
        piv = transform.parent.GetChild(2);
        if (Physics.Raycast(stand.position, Vector3.down, out RaycastHit hit, 100f, Targets.groundLayer)) piv.position = hit.point;
    }

    public IEnumerator enable()
    {
        enb = true;
        for(int i = 0; i<=20; i++)
        {
            pivpos = piv.position;
            // transform.rotation = Quaternion.Euler(*9/40f, 180, 0);
            transform.rotation = Quaternion.Euler((i-20) * (20 - i) * 9 / 40f, 180, 0);
            transform.position += pivpos - piv.position;
            yield return null;
        }
    }
    public IEnumerator disable()
    {
        for (int i = 0; i <= 20; i++)
        {
            pivpos = piv.position;
            transform.rotation = Quaternion.Euler(
                (i - 40) * i * 9 / 40f, 180, 0
            );
            transform.position += pivpos - piv.position;
            yield return null;
        }
        enb = false;
    }
}
