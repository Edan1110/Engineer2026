
using UnityEngine;
using System.Collections;


public class Bow : MonoBehaviour
{
    [SerializeField] Transform arrowBox, arrowPref;
    float loadf;
    Transform topStr, botStr, mid, torustop, torusbot, midtop, midbot, arrowtr;
    Arrow arrow = null;
    Animator bowAnim, arrowAnim;
    readonly int bowload = Animator.StringToHash("load"), load2 = Animator.StringToHash("Load2"), shoot = Animator.StringToHash("shoot");



    void Start()
    {
        torustop = transform.GetChild(1);
        torusbot = transform.GetChild(3);
        topStr = transform.GetChild(4);
        botStr = transform.GetChild(5);
        mid = transform.GetChild(2);
        midtop = mid.GetChild(0);
        midbot = mid.GetChild(1);
        bowAnim = GetComponent<Animator>();   
        arrowAnim = arrowBox.GetComponent<Animator>();

    }
    public bool isNull(){return arrow==null;}
    public IEnumerator Load(bool loadmod2)
    {
        Debug.Log("Loading");
        arrowtr = Instantiate(arrowPref, arrowBox);
        arrowtr.localPosition = Vector3.zero;
        arrowtr.localEulerAngles = Vector3.back * 80;
        // Debug.Log(arrow);
        if(arrow == null) arrow = arrowtr.GetComponent<Arrow>();

        bowAnim.SetBool(bowload, true);
        arrowAnim.SetBool(bowload, true);
        if(loadmod2) arrowAnim.SetTrigger(load2);

        yield return new WaitUntil(()=>arrowAnim.GetBool(LoadBehavior.loadComplete));
        Debug.Log("Load!");
        if(arrow != null) arrow.state = ArrowState.load;
    }

    public void Unload()
    {
        bowAnim.SetBool(bowload, false);
        arrowAnim.SetBool(bowload, false);
        arrowtr.SetParent(null, true);
        StartCoroutine(recoverString(10));
        arrow.Unload();
        arrow = null;
    }

    public void Shoot(float fcoef)
    {
        if(arrow.state == ArrowState.load){
            arrowtr.SetParent(null, true);
            arrow.Shoot(loadf*fcoef);
            StartCoroutine(recoverString(10));
            bowAnim.SetTrigger(shoot);
            bowAnim.SetBool(bowload, false);
            arrowAnim.SetBool(bowload, false);
            arrow = null;
        }
    }
    
    IEnumerator recoverString(int k)
    {
        Vector3 s = mid.localPosition, d = Vector3.right * 6;
        for(int i = 0; i<k; i++)
        {
            setMidPos(mid.parent.TransformPoint(Vector3.Lerp(s, d, i/(k-1f))));
            yield return null;
        }
    }
    void SetCylinderBetweenPoints(Transform cylinder, Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;
        float length = dir.magnitude;

        cylinder.position = (a + b) * 0.5f;

        if (length > 0.0001f) cylinder.rotation = Quaternion.FromToRotation(Vector3.up, dir);

        cylinder.localScale = new Vector3(0.2f/(1+length), length * 12.5f, 0.2f/(1+length));
    }
    void setMidPos(Vector3 pos)
    {
        mid.position = pos;
        SetCylinderBetweenPoints(topStr, midtop.position, torustop.position);
        SetCylinderBetweenPoints(botStr, midbot.position, torusbot.position);  
    }
    public void setLoad(float x)
    {
        loadf = x;
        arrowtr.localPosition = Vector3.back * x;
        if(arrow.state == ArrowState.load)setMidPos(arrowtr.position);
    }

}
