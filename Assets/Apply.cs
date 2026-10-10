using UnityEngine;

public class Apply : MonoBehaviour
{
    [SerializeField] Config c;
    public void a(){
        c.Load();
    }

}
