using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using System.Collections.Generic;
using System;

public class item : MonoBehaviour
{
    private string name;
    private string function;

    public string getFunction()
    {
        return this.function;
    }
}