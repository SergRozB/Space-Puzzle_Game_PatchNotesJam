using System.Collections.Generic;
using UnityEngine;

public class PlanetPrefabStorer : MonoBehaviour
{
    public GameObject[] planetPrefabs;
    public static Dictionary<string, GameObject> planetPrefabDictionary;
    void Awake()
    {
        planetPrefabDictionary = new Dictionary<string, GameObject>();
        foreach (GameObject planet in planetPrefabs)
        {
            planetPrefabDictionary.Add(planet.name, planet);
        }
    }
}
