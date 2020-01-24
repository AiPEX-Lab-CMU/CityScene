using System;
using UnityEngine;

public class Box
{
    string name;
    public Vector3 position;

    public Box(string name)
    {
        this.name = name;
        position = new Vector3();
    }
    public Box()
    {
        name = "";
        position = new Vector3();
    }

    public string getName()
    {
        return name;
    }
}
