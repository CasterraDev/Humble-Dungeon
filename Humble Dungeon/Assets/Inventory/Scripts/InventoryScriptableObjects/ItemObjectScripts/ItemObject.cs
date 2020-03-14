using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Food,
    Equipment,
    Default
}

public abstract class ItemObject : ScriptableObject
{
    public int Id;
    public GameObject realWorldPrefab;
    public Sprite sprite;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
}

[System.Serializable]
public class Item
{
    public int Id;

    public GameObject realWorldPrefab;
    public string name;

    public Item(ItemObject item)
    {
        Id = item.Id;
        realWorldPrefab = item.realWorldPrefab;
        name = item.name;
    }
}
