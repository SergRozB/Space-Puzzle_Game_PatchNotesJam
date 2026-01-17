using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    bool inProgress = true;
    private Inventory inventory;
    List<Planet> planets = new List<Planet>.list();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < planets.Count; i++)
        {
            Vector2 force = getGravityVector(planets[i]);
            player.transform.position(force);
        }
    }

    /*
    Vector2 getGravityVector(Planet planet)
    {
        int distance = sqrt(abs(planet.getx() - player.getx()) ** 2 + (abs(planet.gety() - player.gety()) **2)
        int totalForce = planet.getMass() / distance^2
        Vector2 force = new Vector2((planet.getx() - player.getx()) * gravityForce, (planet.getx() - player.getx()) * gravityForce)
        return force
    }
    */



    void FixedUpdate()
    {
        Vector2 movement = MovementFunc();
        transform.Translate(new Vector2(movement.x, movement.y));
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "planet")
        {
            Debug.Log("Collided with planet");
            inProgress = false;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "item")
        {
            // get item;
        }
    }

    void destroyCoin(GameObject item) { 
        Destroy(item);
    }
}
