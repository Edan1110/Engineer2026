using System.Reflection;
using UnityEngine;

public class Targets : MonoBehaviour
{
    Archery[] tar = new Archery[9];
    public static int groundLayer;
    public Transform scarecrow;
    int a, b;
    // float t = 0;
    bool newenb = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
        for(int i = 0; i<9; i++)
        {
            tar[i] = transform.GetChild(i).GetChild(1).GetComponent<Archery>();
            tar[i].transform.rotation = Quaternion.Euler(-90, 180, 0);
            StartCoroutine(tar[i].disable());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!P.toStart) return;
        // t += Time.deltaTime;
        if (newenb)
        {
            if (Random.value < 0.2f)
            {
                Instantiate(scarecrow).GetComponent<Scarecrow>().emerge();

            }
            // t = 0f;
            a = Random.Range(0, 9);
            b = Random.Range(0, 8);
            if (b >= a) b++;
            StartCoroutine(tar[a].enable());
            StartCoroutine(tar[b].enable());
            newenb = false;
        }
        else if(!tar[a].enb && !tar[b].enb) newenb = true;
    }
}
