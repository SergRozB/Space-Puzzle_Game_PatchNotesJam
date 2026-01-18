using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;
using System;

public class Item : MonoBehaviour
{
    private string name;
    private string function;

    public string getFunction()
    {
        return this.function;
    }
}