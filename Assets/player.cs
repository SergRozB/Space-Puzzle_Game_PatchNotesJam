using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System;
using System.IO;


public class Player : MonoBehaviour
{
    bool inProgress = true;
    private Vector3 prevPostition = new Vector3(){};
    private List<GameObject> trajObjects = new List<GameObject>(){};
    [SerializeField] private GameObject trajectoryPosition;
    [SerializeField] private const int MAXTRAJECTORY = 50;
    private Inventory inventory;
    private bool fired = false;
    private Vector2 mousePos = new Vector2(){};
    [SerializeField] private Vector3 forwardVel = new Vector3(0f, 0f, 0f){};
    [SerializeField] private float friction = 0.95f;
    private Vector2 prevMousePos = new Vector2(){};
    [SerializeField] float MAXGRAVFORCE = 0.5f;
    [SerializeField] float exponent = 2.8f;
    Mouse mouse = Mouse.current;
    public List<Planet> planets = new List<Planet>{};
    [SerializeField] public float projectionVel = 0.5f;
    public bool fireRequest = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Update()
    {
        if (mouse.leftButton.wasPressedThisFrame)
        {    
            fireRequest = true;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        mousePos = mouse.position.ReadValue();
        if (fired != true)
        {   
            if (fireRequest)
            {
                fireRequest = false;
                fired = true;
                forwardVel = getVel(transform.position, mouseToWorld()) * projectionVel;

            } else {               
                if (mouse.position.ReadValue() != prevMousePos) {
                    foreach(GameObject obj in trajObjects) {
                        Destroy(obj);
                    }

                    updateTrajectory();
                }
            }
        } else {
            updatePlayer();
        }

        prevMousePos = mousePos;
    }

    Vector3 mouseToWorld()
    {
        Vector3 m = mousePos;
        m.z = Camera.main.WorldToScreenPoint(transform.position).z;
        return Camera.main.ScreenToWorldPoint(m);
    }

    Vector3 getVel(Vector3 prevVel, Vector3 newVel)
    {
        Vector3 v = newVel - prevVel;
        return v.normalized;
    }
    void updateTrajectory()
    {
        Vector3 position = transform.position;
        Vector3 trajVel = forwardVel;
        Vector3 mouseWorldPos = mouseToWorld();

        forwardVel = getVel(transform.position, mouseWorldPos) * projectionVel;

        for (int i = 0; i < 40; i++) {
            prevPostition = position;

            for (int j = 0; j < planets.Count; j++)
            {
                Vector2 force = getGravityVector(planets[j], position);
                position += new Vector3 (force.x, force.y, 0);
            }

            position += trajVel;
            trajVel = getVel(prevPostition, position) * friction;

            if (i % 4 == 0)
            {
                GameObject trajObject = Instantiate(trajectoryPosition, position, transform.rotation);
                trajObjects.Add(trajObject);
            }
        } 
    }
    void updatePlayer()
    {
        prevPostition = transform.position;
        for (int i = 0; i < planets.Count; i++)
        {
            Vector2 force = getGravityVector(planets[i], transform.position);
            transform.position += new Vector3 (force.x, force.y, 0);
        }

        transform.position += forwardVel;
        forwardVel = getVel(prevPostition, transform.position) * friction;
    }
    
    Vector2 getGravityVector(Planet planet, Vector3 position)
    {
        Vector2 force = new Vector2(){};
        double distance = Math.Sqrt(Math.Pow(Math.Abs(planet.getX() - position.x), 2) + Math.Pow(Math.Abs(planet.getY() - position.y), 2));
        double totalForce = planet.getMass() / Math.Pow(distance, exponent);

        if (totalForce > 0)
        {
            force = new Vector2((float)(planet.getX() - position.x), (float)(planet.getY() -position.y)).normalized * (float)totalForce;

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
            Destroy(collision.gameObject);
            inventory.addItem(collision.gameObject);
        }

        if (collision.gameObject.tag == "goal")
        {
            Debug.Log("you win");
            inProgress = false;
        }
    }
}
