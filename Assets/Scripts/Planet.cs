using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Planet : MonoBehaviour
{

    [SerializeField] private float mass = 1.5f;
    [SerializeField] private Vector3 vel = new Vector3(0, 0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Planet(float mass, Vector3 vel)
    {
        this.mass = mass;
        this.vel = vel;
    }

    public GameObject getGameObject()
    {
        return this.gameObject;
    }
    public double getMass()
    {
        return this.mass;
    }

    public double getX()
    {
        return transform.position.x;
    }

    public double getY()
    {
        return transform.position.y;
    }

    public float getVelX()
    {
        return this.vel.x;
    }

    public float getVelY()
    {
        return this.vel.y;
    }

    public void setVel(Vector3 vel)
    {
        this.vel = vel;
    }

    public void setMass(float mass)
    {
        this.mass = mass;
    }
}
