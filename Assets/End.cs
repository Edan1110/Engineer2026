using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    Keyboard k;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        k = Keyboard.current;
        GetComponent<TMP_Text>().text = "Clear!\nScore: "+ Player.score;
    }
    void Update()
    {
        if (k.sKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene("Config");
        }
    }

}
