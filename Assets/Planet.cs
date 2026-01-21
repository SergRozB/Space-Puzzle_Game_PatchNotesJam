using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Planet : MonoBehaviour
{

    [SerializeField] private double mass = 1.5;
    [SerializeField] private Vector3 vel = new Vector3(0, 0, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public Vector3 getVel()
    {
        return this.vel;
    }
}
