using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Security.Cryptography;

public class Inventory : MonoBehaviour
{
    private List<int> inventory = new List<int>(){};

    public Inventory(List<int> initialValues)
    {
        this.inventory = initialValues;
    }
    public void addItem(int item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            this.inventory.Add(item);
        }
        
    }

    public void setItems(List<int> values)
    {
        this.inventory = values;
    }
}