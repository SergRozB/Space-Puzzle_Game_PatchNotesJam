using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    private List<int> inventory = new List<int>(){};

    public void addItem(int item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            inventory.Add(item);
        }
        
    }
}