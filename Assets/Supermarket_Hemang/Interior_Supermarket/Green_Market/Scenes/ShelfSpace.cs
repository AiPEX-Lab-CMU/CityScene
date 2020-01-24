using System;
using System.Collections.Generic;
using UnityEngine;

public class ShelfSpace
{
    string name;
    public int capacity;
    public List<Vector3> itemPositions;
    public ShelfSpace(string name)
    {
        this.name = name;
        capacity = 1;
        itemPositions = new List<Vector3>();
    }
}