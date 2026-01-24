using UnityEngine;

public class TrajectoryObjectScript : MonoBehaviour
{
    public bool applyGravity = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("GravCircle"))
        {
            applyGravity = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GravCircle"))
        {
            applyGravity = false;
        }
    }
}
