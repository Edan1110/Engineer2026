using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Config : MonoBehaviour
{
    public static string port = "COM4";
    public static float[] fdata; 
    public static int[] idata;


    [SerializeField]GameObject inputfield;
    [SerializeField]string[] fdataname, idataname;

    GameObject temp;
    static TMP_InputField[] inputs;
    static bool init = true;
    static int fleng, ileng;

    // Arrow.rb
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        if (init)
        {
            fleng = fdataname.Length;
            ileng = idataname.Length;
            fdata = new float[]{10, 0, 6, 0, -6, 2e-4F, 0.015f, 1.1f, 0.001f, 1, 1000, 800};
            // fdata = new float[fleng];
            idata = new int[]{5, 60};
            init = false;
            inputs = new TMP_InputField[fleng + ileng + 1];
        }

        temp = Instantiate(inputfield, transform);
        temp.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        inputs[0] = temp.GetComponent<TMP_InputField>();
        inputs[0].placeholder.GetComponent<TextMeshProUGUI>().text = "port";
        inputs[0].text = port;
        for(int i = 0; i<fleng; i++)
        {
            temp = Instantiate(inputfield, transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(250, i * -50);
            inputs[i+1] = temp.GetComponent<TMP_InputField>();
            inputs[i+1].placeholder.GetComponent<TextMeshProUGUI>().text = fdataname[i];
            inputs[i+1].text = fdata[i].ToString(); 
        }
        for(int i = 0; i<ileng; i++)
        {
            temp = Instantiate(inputfield, transform);
            temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(500, i * -50);
            inputs[i+1+fleng] = temp.GetComponent<TMP_InputField>();
            inputs[i+1+fleng].placeholder.GetComponent<TextMeshProUGUI>().text = idataname[i];
            inputs[i+1+fleng].text = idata[i].ToString(); 
        }
    }
    public void Load()
    {
        port = inputs[0].text;
        for(int i = 0; i<fleng; i++) fdata[i] = float.Parse(inputs[i+1].text);
        for(int i = 0; i<ileng; i++) idata[i] = int.Parse(inputs[i+1+fleng].text);
        SceneManager.LoadScene("MainScene");
    }

}
