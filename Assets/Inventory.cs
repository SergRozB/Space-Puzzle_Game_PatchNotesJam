using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private List<GameObject> inventory = new List<GameObject>(){};

    public void addItem(GameObject item)
    {
        inventory.Add(item);
    }
}