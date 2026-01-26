using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;
using System;

public class Item : MonoBehaviour
{
    [SerializeField] private string attributeName;

    public GameObject getGameObject()
    {
        return this.gameObject;
    }
    
    public string getAttributeName()
    {
        return this.attributeName;
    }

    public void setName(string name)
    {
        this.attributeName = name;
    }
}