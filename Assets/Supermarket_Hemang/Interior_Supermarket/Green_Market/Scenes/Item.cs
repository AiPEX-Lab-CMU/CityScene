using System;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    string name;
    public int count;
    //public Dictionary<string, Vector3> positions;
    public List<string> objectNames;
    public List<Vector3> positions;
    public List<string> parentName;
    public bool hasBox;
    //public Box container;
    public Dictionary<string, Box> containers;
    public Item(string name)
    {
        this.name = name;
        count = 1;
        //positions = new Dictionary<string, Vector3>();
        objectNames = new List<string>();
        positions = new List<Vector3>();
        parentName = new List<string>();
        hasBox = false;
        containers = new Dictionary<string, Box>();
    }
    public string getName()
    {
        return name;
    }
}
