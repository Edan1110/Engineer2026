using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{
    SC x;
    // Image i;
    RectTransform i;
    // public GameObject g;
    Keyboard k;
    public float t;
    public bool pause;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x = new SC(t, .94f, 120, 0.01f);
        i = GetComponent<RectTransform>();
        // GetComponent<Image>
        k = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pause){
            x.update(t);
            Debug.Log(x.value);
            i.anchoredPosition = Vector3.right*x.value * 10;
        }
    }
}
