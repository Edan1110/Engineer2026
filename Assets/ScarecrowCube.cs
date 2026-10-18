using UnityEngine;

public class ScarecrowCube : MonoBehaviour, Target
{
    public void onHit(ContactPoint contact,Arrow arrow)
    {
        transform.parent.GetComponent<Target>().onHit(contact, arrow);
    }
}
