using UnityEngine;

public class GravityCircleScript : MonoBehaviour
{
    public bool applyGravity = false;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        applyGravity = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        applyGravity = false;
    }
}
