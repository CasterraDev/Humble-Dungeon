using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building System/Building")]
public class BuildableObject : ScriptableObject
{
    public int Id;
    public GameObject prefab;
    public BuildingType type = BuildingType.Ground;
    public bool makeGrounded = true;
}

public enum BuildingType
{
    Ground,
    Wall,
}
