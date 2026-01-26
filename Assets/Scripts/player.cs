using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(UnityEngine.UI.Slider))]
public class Player : MonoBehaviour
{
    bool inProgress = true;
    private Vector3 prevPostition = new Vector3(){};
    private List<GameObject> trajObjects = new List<GameObject>(){};
    [SerializeField] private GameObject trajectoryPosition;
    [SerializeField] private GameObject itemboxObj;
    [SerializeField] private GameObject itemObj;
    [SerializeField] private GameObject sliderObj;
    [SerializeField] private Transform canvas;
    private UnityEngine.UI.Slider slider;
    [SerializeField] private const int MAXTRAJECTORY = 50;
    private List<GameObject> inventory = new List<GameObject>(){};
    public bool fired = false;
    private Vector2 mousePos = new Vector2(){};
    [SerializeField] private Vector3 forwardVel = new Vector3(0f, 0f, 0f){};
    [SerializeField] private float friction = 0.95f;
    private Vector2 prevMousePos = new Vector2(){};
    [SerializeField] float MAXGRAVFORCE = 0.5f;
    [SerializeField] float exponent = 2.8f;
    Mouse mouse = Mouse.current;
    public GameObject allPlanetsParent;
    private List<Planet> planets = new List<Planet>{};
    public List<ItemBox> itemBoxes = new List<ItemBox>{};
    [SerializeField] public float projectionVel = 0.5f;
    private bool fireRequest = false;
    private bool paused = false;
    private string filePath;
    private bool userChangeSlider = true;
    [SerializeField] private int maxTrajectoryPoints = 10;
    public float startTime = 0;
    public float currentTime = 0;
    private List<GameObject> planetsOrbiting = new List<GameObject>();
    private Animator animator;
    [SerializeField] private GameObject deadScreen;
    [SerializeField] private LayerMask trajCollisionMask;

    void Awake()
    {
        animator = GetComponent<Animator>();
        planets = allPlanetsParent.GetComponentsInChildren<Planet>().ToList();
        filePath = Path.Combine(Application.persistentDataPath, "history.csv");
        for(int i = 0; i < maxTrajectoryPoints; i++) {
            GameObject trajObject = Instantiate(trajectoryPosition, new Vector3(0,0,0), transform.rotation);
            trajObjects.Add(trajObject);
        }

        GameObject s = Instantiate(sliderObj, new Vector3(0, 0, 0), transform.rotation, canvas);
        slider = s.GetComponent<UnityEngine.UI.Slider>();
        RectTransform sliderRect = slider.GetComponent<RectTransform>();
        sliderRect.anchoredPosition = new Vector3(0, 120, 0);
        sliderRect.sizeDelta = new Vector3(400f, 20f, 0f);
        slider.wholeNumbers = true;
        slider.minValue = 0;
        slider.maxValue = 0;
        slider.value = 0;
        slider.onValueChanged.AddListener(delegate { sliderValueChanged(); });
    }

    void Start()
    {
        File.WriteAllText(filePath, string.Empty);
    }

    void Update()
    {
        if (!paused && fired)
        {
            currentTime += Time.deltaTime;
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {    
            fireRequest = true;
        }

        if (mouse.rightButton.wasPressedThisFrame)
        {
            var lines = File.ReadLines(filePath).Take((int)slider.value + 1).ToList();
            File.WriteAllLines(filePath, lines);
            slider.maxValue = slider.value + 1;

            paused = false;
        }

        if (gameObject.GetComponent<PlayerInputManager>().pauseInGameTime)
        {
            paused = !paused;
        }

        RotateToVelocity();
    }

    void FixedUpdate()
    {
        mousePos = mouse.position.ReadValue();
        if (!paused) {
            if (!fired)
            {   
                if (fireRequest)
                {
                    if (trajObjects.Count != 0)
                    {
                        foreach (GameObject obj in trajObjects)
                        {
                            Destroy(obj);
                        }
                    }
                    fireRequest = false;
                    fired = true;
                    forwardVel = getVel(transform.position, mouseToWorld()) * projectionVel;
                    startTime = Time.time;  

                } else {               
                    if (mouse.position.ReadValue() != prevMousePos) {

                        updateTrajectory();
                    }
                }
            } else {
                updatePlayer();
            }
        } 
        prevMousePos = mousePos;
    }
    void sliderValueChanged()
    {
        if (userChangeSlider)
        {
            loadPosition(File.ReadLines(filePath).Skip((int)slider.value - 1).Take(1).First());
            paused = true;
        }
    }

    private void RotateToVelocity() 
    {
        float angleToRotate = Vector3.Angle(Vector3.right, forwardVel);
        if(forwardVel.y < 0) 
        {
            angleToRotate = -angleToRotate;
        }
        transform.localEulerAngles = new Vector3(0, 0, angleToRotate);
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
        Vector3 mouseWorldPos = mouseToWorld();

        forwardVel = getVel(transform.position, mouseWorldPos) * projectionVel;
        Vector3 trajVel = forwardVel;
        bool pathEndedEarly = false;

        for (int i = 0; i < maxTrajectoryPoints; i++) {
            prevPostition = position;
            GameObject currentTrajObject = trajObjects[i];
            TrajectoryObjectScript trajScript = currentTrajObject.GetComponent<TrajectoryObjectScript>();
            BoxCollider2D markerCollider = currentTrajObject.GetComponent<BoxCollider2D>();

            Vector3 nextPos = position + trajVel;

            float angleToRotate = Vector3.Angle(Vector3.right, trajVel);
            if (trajVel.y < 0)
            {
                angleToRotate = -angleToRotate;
            }
            currentTrajObject.transform.localEulerAngles = new Vector3(0, 0, angleToRotate);

            if (Physics2D.OverlapBox((Vector2)nextPos, markerCollider.size, angleToRotate, trajCollisionMask))
            {
                pathEndedEarly = true;
                Debug.Log($"Trajectory ended early at point {i} (physics overlap)");
            }
     
            if (!pathEndedEarly)
            {
                currentTrajObject.SetActive(true);

                for (int j = 0; j < planets.Count; j++)
                {
                    Planet planet = planets[j];


                    if (trajScript.planetsOrbiting.Contains(planet.gameObject))
                    {
                        Vector2 force = getGravityVector(planet, position);
                        position += new Vector3(force.x, force.y, 0);
                    }
                }

                position += trajVel;
                trajVel = getVel(prevPostition, position) * friction;
                currentTrajObject.transform.position = position;
            }

            else 
            {
                currentTrajObject.SetActive(false);
            }
        } 
    }

    void updatePlayer()
    {
        prevPostition = transform.position;
        for (int i = 0; i < planets.Count; i++)
        {
            Planet planet = planets[i];

            if (planetsOrbiting.Contains(planet.gameObject))
            {
                Vector2 force = getGravityVector(planet, transform.position);
                transform.position += new Vector3(force.x, force.y, 0);
            }
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
        float counter = 0f;
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            outLine += transform.position.x.ToString() + "," + 
            transform.position.y.ToString() + "," + 
            prevPostition.x.ToString() + "," + 
            prevPostition.y.ToString() + "," + 
            forwardVel.x.ToString() + "," + 
            forwardVel.y.ToString() + "," + 
            friction + "," + 
            itemBoxes.Count.ToString() + ",";
            foreach (ItemBox box in itemBoxes)
            {
                outLine += box.getX().ToString() + "," + 
                box.getY().ToString() + "," + 
                box.getItem().getName() + "," +
                box.getItemAmount().ToString() + ",";
            }

            outLine += planets.Count.ToString() + ",";

            foreach (Planet planet in planets)
            {
                outLine += planet.getX().ToString() + "," + 
                planet.getY().ToString() + "," + 
                planet.getVelX().ToString() + "," + 
                planet.getVelY().ToString() + "," + 
                planet.getMass().ToString() + "," +
                planet.name + ",";
            }

            outLine += currentTime;

            writer.WriteLine(outLine);
            slider.maxValue += 1;
            userChangeSlider = false;
            slider.value = slider.maxValue;
            userChangeSlider = true;
        }
    }

    void loadPosition(string frame) 
    {
        // transform.x, transform.y, prevPosition.x, prevPosition.y, forwardVel.x, forwardVel.y, friction, i1amount, i2amount... i9amount, itemBoxAmount, itemBox1.x, itemBox1.y, itemBox1.item, itemBox1.itemamount..., objAmount, obj1.x, obj1.y, obj1.vel.x, obj1.vel.y, obj.mass...
        
        reloadSave();
        string[] frameInfo = frame.Split(',');

        transform.position = new Vector3(stringToFloat(frameInfo[0]), stringToFloat(frameInfo[1]), 0);
        prevPostition = new Vector3(stringToFloat(frameInfo[2]), stringToFloat(frameInfo[3]), 0);
        forwardVel = new Vector3(stringToFloat(frameInfo[4]), stringToFloat(frameInfo[5]), 0);
        friction = stringToFloat(frameInfo[6]);

        if (int.Parse(frameInfo[7]) != 0) { 
            for (int i = 0; i < int.Parse(frameInfo[7]); i++)
            {
                PlayerInputManager  playerInputManager = GetComponent<PlayerInputManager>();
                List<Vector3> itemBoxPositions = playerInputManager.itemBoxPositions;
                Vector3 itemBoxPos = new Vector3(stringToFloat(frameInfo[8 + 4 * i]), stringToFloat(frameInfo[9 + 4 * i]), 0);
                if (!itemBoxPositions.Contains(itemBoxPos))
                {
                    GameObject gameBox = Instantiate(itemboxObj, new Vector3(stringToFloat(frameInfo[8 + 4 * i]), stringToFloat(frameInfo[9 + 4 * i]), 0), Quaternion.identity);
                    ItemBox itembox = gameBox.AddComponent<ItemBox>();
                    GameObject gameItem = Instantiate(itemObj, new Vector3(stringToFloat(frameInfo[8 + 4 * i]), stringToFloat(frameInfo[9 + 4 * i]), 0), Quaternion.identity);
                    Item item = gameItem.AddComponent<Item>();
                    item.setName("forward");
                    itembox.setItem(item);
                    itembox.setItemAmount(Int32.Parse(frameInfo[11 + 4 * i]));
                    itemBoxes.Add(itembox);
                }
            }
        }

        int csvIndex = 8 + int.Parse(frameInfo[7]) * 4;

        if (int.Parse(frameInfo[csvIndex]) != 0) {
            for (int i = 0; i < int.Parse(frameInfo[csvIndex]); i++)
            {
                GameObject prefabToUse = PlanetPrefabStorer.planetPrefabDictionary[frameInfo[csvIndex + 6 + 6*i]];
                GameObject gamePlanet = Instantiate(prefabToUse, new Vector3(stringToFloat(frameInfo[csvIndex + 1 + 6*i]), stringToFloat(frameInfo[csvIndex + 2 + 6*i]), 0), Quaternion.identity, allPlanetsParent.transform);
                Planet planet = gamePlanet.AddComponent<Planet>();
                planet.setVel(new Vector3(stringToFloat(frameInfo[csvIndex + 3 + 6*i]), stringToFloat(frameInfo[csvIndex + 4 + 6*i]), 0));
                planet.setMass(stringToFloat(frameInfo[csvIndex + 5 + 6*i]));
                planet.name = prefabToUse.name;
                planets.Add(planet);
            }
        }

        currentTime = stringToFloat(frameInfo[csvIndex + 1 + int.Parse(frameInfo[csvIndex]) * 6]);
    }

    float stringToFloat(string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
    }

    void reloadSave()
    {
        foreach (Planet planet in planets) {
            Destroy(planet.getGameObject());
        }

        foreach (ItemBox box in itemBoxes)
        {
            Destroy(box.getItem().getGameObject());
            Destroy(box.getGameObject());
        }

        planets.Clear();
        itemBoxes.Clear();

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
            inventory.Add(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Planet"))
        {
            animator.SetBool("collided", true);
            deadScreen.SetActive(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("GravCircle"))
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
}
