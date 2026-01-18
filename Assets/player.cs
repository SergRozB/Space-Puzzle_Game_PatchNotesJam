using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;


public class Player : MonoBehaviour
{
    bool inProgress = true;
    private Inventory inventory;
    private bool fired = false;
    private Vector2 mousePos = new Vector2(){};
    [SerializeField] private Vector3 initialVel = new Vector3(0f, 0f, 0f){};
    [SerializeField] float MAXGRAVFORCE = 0.5f;
    [SerializeField] float friction = 0.9875f;
    [SerializeField] float exponent = 2.8f;
    public List<Planet> planets = new List<Planet>{};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (fired != true)
        {
            var mouse = Mouse.current;
            mousePos = mouse.position.ReadValue();

            float xdist = math.abs(transform.position.x - mousePos.x);
            float ydist = math.abs(transform.position.y - mousePos.y);
            float ratio = xdist / (xdist + ydist);
            Debug.Log(ratio);

            initialVel = new Vector3(0.1f * ratio, 0.1f * (1 - ratio), 0);
            
            if (mouse.leftButton.wasPressedThisFrame)
            {
                fired = true;
            }
        } else {
            for (int i = 0; i < planets.Count; i++)
            {
                Vector2 force = getGravityVector(planets[i]);
                transform.position += new Vector3 (force.x, force.y, 0);

                transform.position += initialVel;
            }

            initialVel *= friction;
        }
    }

    
    Vector2 getGravityVector(Planet planet)
    {
        Vector2 force = new Vector2(){};
        double distance = Math.Sqrt(Math.Pow(Math.Abs(planet.getX() - transform.position.x), 2) + Math.Pow(Math.Abs(planet.getY() - transform.position.y), 2));
        double totalForce = planet.getMass() / Math.Pow(distance, exponent);

        if (totalForce > 0)
        {
            force = new Vector2((float)(planet.getX() - transform.position.x), (float)(planet.getY() -transform.position.y)).normalized * (float)totalForce;

            if (force.magnitude > Math.Abs(Math.Sqrt(2 * MAXGRAVFORCE))) 
            {
                force = new Vector2(MAXGRAVFORCE, MAXGRAVFORCE);
            }
        }
        return force;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "planet")
        {
            Debug.Log("Collided with planet");
            inProgress = false;
        }

        if (collision.gameObject.tag == "itemBox")
        {
            Destroy(this.gameObject);
            inventory.addItem(collision.gameObject);
        }
    }
}
