using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;
using System;

public class Item : MonoBehaviour
{
    [SerializeField] private string name;

    public GameObject getGameObject()
    {
        return this.gameObject;
    }
    
    public string getName()
    {
        return this.name;
    }

    public void setName(string name)
    {
        this.name = name;
    }
}