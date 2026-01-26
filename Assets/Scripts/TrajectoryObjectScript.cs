using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryObjectScript : MonoBehaviour
{
    public List<GameObject> planetsOrbiting = new List<GameObject>();
    private GameObject player;
    private BoxCollider2D boxCollider;
    public bool destroyCollisionDetected = false;   

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        BoxCollider2D playerBoxCollider = player.GetComponent<BoxCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = playerBoxCollider.size;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("GravCircle"))
        {
            planetsOrbiting.Add(collision.gameObject.transform.parent.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GravCircle"))
        {
            planetsOrbiting.Remove(collision.gameObject.transform.parent.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Planet") || collision.gameObject.CompareTag("Obstacle"))
        {
            destroyCollisionDetected = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Planet") || collision.gameObject.CompareTag("Obstacle"))
        {
            destroyCollisionDetected = false;
        }
    }
}
