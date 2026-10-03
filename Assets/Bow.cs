using System;
using UnityEditor.Rendering;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public Transform camera, arrowBox, arrowPref;
    Arrow arrow = null;

    Animator bowAnim, arrowAnim;
    public bool load = false;
    bool temp  = false;
    public bool loadmod2 = false;
    static readonly int bowload = Animator.StringToHash("load");   
    static readonly int load2 = Animator.StringToHash("Load2");   
    Transform arrowtr;


    void Start()
    {
        bowAnim = GetComponent<Animator>();   
        arrowAnim = arrowBox.GetComponent<Animator>();

    }
    void Load()
    {
        arrowtr = Instantiate(arrowPref, arrowBox);
        arrowtr.localPosition = Vector3.zero;
        arrowtr.localEulerAngles = Vector3.back * 80;
        // Debug.Log(arrow);
        if(arrow == null) arrow = arrowtr.GetComponent<Arrow>();

        bowAnim.SetBool(bowload, true);
        arrowAnim.SetBool(bowload, true);
        if(loadmod2) arrowAnim.SetTrigger(load2);
        // arrowAnim.SetBool(loadnum, !arrowAnim.GetBool(loadnum))
        // else if(!b && arrow != null) ;
    }
    void Unload()
    {
        bowAnim.SetBool(bowload, false);
        arrowAnim.SetBool(bowload, false);
        arrow.transform.SetParent(null, true);
        arrow.Unload();
        arrow = null;
    }
    // Update is called once per frame
    void Update()
    {
        // bowAnim.SetBool(bowload, load);
        if(!temp && load)
        {
            Load();
        }
        else if(temp && !load)
        {
            Unload();
        }
        temp = load;
        // transform.position = camera.position + camera.forward *1f;
        // transform.rotation = camera.rotation;
        
    }
}
