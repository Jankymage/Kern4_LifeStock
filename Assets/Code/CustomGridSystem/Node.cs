using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWater;
    public bool hasMineral;
    public bool clickedOn;
    public bool walkable;
    public bool isCloseToWater;
    public bool theresBuilding;
    public bool theresGhost;
    public Vector3 worldPosition;

    public Node(bool _walkable, Vector3 _worldPos){
        walkable = _walkable;
        worldPosition = _worldPos;
    }
}
