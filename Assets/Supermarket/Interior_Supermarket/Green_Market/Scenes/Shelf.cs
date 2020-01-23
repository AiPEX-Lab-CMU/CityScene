using System;
using System.Collections.Generic;
using UnityEngine;
public class Shelf
{
    string name;
    public Vector3 position;
    public Dictionary<string, Item> items;
    public Shelf(string name)
    {
        this.name = name;
        position = new Vector3();
        items = new Dictionary<string, Item>();
    }

    public string getName()
    {
        return name;
    }
}
