using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Unity.Mathematics;
using System.IO;
using System.Globalization;
using System.Linq;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    bool inProgress = true;
    private Vector3 prevPostition = new Vector3(){};
    private List<GameObject> trajObjects = new List<GameObject>(){};
    [SerializeField] private GameObject trajectoryPosition;
    [SerializeField] private const int MAXTRAJECTORY = 50;
    private Inventory inventory = new Inventory(new List<int>(){0, 0, 0, 0, 0, 0, 0, 0, 0});
    private bool fired = false;
    private Vector2 mousePos = new Vector2(){};
    [SerializeField] private Vector3 forwardVel = new Vector3(0f, 0f, 0f){};
    [SerializeField] private float friction = 0.95f;
    private Vector2 prevMousePos = new Vector2(){};
    [SerializeField] float MAXGRAVFORCE = 0.5f;
    [SerializeField] float exponent = 2.8f;
    Mouse mouse = Mouse.current;
    public List<Planet> planets = new List<Planet>{};
    public List<ItemBox> itemBoxes = new List<ItemBox>{};
    [SerializeField] public float projectionVel = 0.5f;
    public bool fireRequest = false;
    private string filePath;

    void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, "history.csv");
    }

    void Start()
    {
        File.WriteAllText(filePath, string.Empty);
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

        for (int i = 0; i < 10; i++) {
            prevPostition = position;

            for (int j = 0; j < planets.Count; j++)
            {
                Vector2 force = getGravityVector(planets[j], position);
                position += new Vector3 (force.x, force.y, 0);
            }

            position += trajVel;
            trajVel = getVel(prevPostition, position) * friction;

            if (i % 1 == 0)
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

        saveposition();
    }
    void saveposition()
    {
        // transform.x, transform.y, prevPosition.x, prevPosition.y, forwardVel.x, forwardVel.y, friction, i1amount, i2amount... i9amount, itemBoxAmount, itemBox1.x, itemBox1.y, itemBox1.item, itemBox1.amount..., objAmount, obj1.x, obj1.y, obj1.vel.x, obj1.vel.y, obj.mass...
        string outLine = "";
        filePath = Path.Combine(Application.persistentDataPath, "history.csv");
        Debug.Log(filePath);
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            outLine += transform.position.x.ToString() + "," + 
            transform.position.y.ToString() + "," + 
            prevPostition.x.ToString() + "," + 
            prevPostition.y.ToString() + "," + 
            forwardVel.x.ToString() + "," + 
            forwardVel.y.ToString() + "," + 
            friction + "," + 
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            "0" + "," +
            itemBoxes.Count.ToString() + ",";

            foreach (ItemBox itemBox in itemBoxes)
            {
                outLine += itemBox.getX().ToString() + "," + 
                itemBox.getY().ToString() + "," + 
                itemBox.getItem().ToString()+ "," +
                itemBox.getItemAmount().ToString() + ",";
            }

            outLine += planets.Count.ToString() + ",";

            foreach (Planet planet in planets)
            {
                outLine += planet.getX().ToString() + "," + 
                planet.getY().ToString() + "," + 
                planet.getVel().x.ToString() + "," + 
                planet.getVel().y.ToString() + "," + 
                planet.getMass().ToString() + ",";
            }

            Debug.Log(outLine);
            writer.WriteLine(outLine);
        }
    }

    void loadPosition() 
    {
        // transform.x, transform.y, prevPosition.x, prevPosition.y, forwardVel.x, forwardVel.y, friction, i1amount, i2amount... i9amount, itemBoxAmount, itemBox1.x, itemBox1.y, itemBox1.item, itemBox1.itemamount..., objAmount, obj1.x, obj1.y, obj1.vel.x, obj1.vel.y, obj.mass...
        filePath = Path.Combine(Application.persistentDataPath, "history.csv");

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    string frame = reader.ReadLine();
                    string[] frameInfo = frame.Split(',');

                    transform.position = new Vector3(stringToFloat(frameInfo[0]), stringToFloat(frameInfo[1]), 0);
                    prevPostition = new Vector3(stringToFloat(frameInfo[2]), stringToFloat(frameInfo[3]), 0);
                    forwardVel = new Vector3(stringToFloat(frameInfo[4]), stringToFloat(frameInfo[5]), 0);
                    friction = stringToFloat(frameInfo[6]);

                    for (int i = 0; i < 9; i++)
                    {
                        inventory.setItems(frameInfo[6..15].Select(int.Parse).ToList());
                    }

                    for (int i = 0; i < int.Parse(frameInfo[16]); i++)
                    {
                        ItemBox box = Instantiate(new ItemBox(int.Parse(frameInfo[18 + 4*i]), int.Parse(frameInfo[19 + 4*i])), new Vector3(stringToFloat(frameInfo[16 + 4*i]), stringToFloat(frameInfo[17 + 4*i]), 0), Quaternion.identity);
                        itemBoxes.Add(box);
                    }

                    int csvIndex = 16 + int.Parse(frameInfo[16]) * 4;

                    for (int i = 0; i < int.Parse(frameInfo[csvIndex]); i++)
                    {
                        Planet planet = Instantiate(new Planet(stringToFloat(frameInfo[csvIndex + 5 + 5*i]), new Vector3(stringToFloat(frameInfo[csvIndex + 3 + 5*i]), stringToFloat(frameInfo[csvIndex + 4 + 5*i]), 0)), new Vector3(stringToFloat(frameInfo[csvIndex + 1 + 5*i]), stringToFloat(frameInfo[csvIndex + 2 + 5*i]), 0), Quaternion.identity);
                        planets.Add(planet);
                    }
                }
            }
        }
    }

    float stringToFloat(string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
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
            //inventory.addItem(collision.gameObject);
        }

        if (collision.gameObject.tag == "goal")
        {
            Debug.Log("you win");
            inProgress = false;
        }
    }
}
