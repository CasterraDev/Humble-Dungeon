using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabase database;
    public int maxItems = 25;
    public int hotBarSlots = 5;
    public Inventory container;

    public void Awake()
    {
        container.items = new InventorySlot[maxItems];
    }

    public void AddItem(Item _item, int _amount)
    {
        for (int i = 0; i < container.items.Length; i++)
        {
            if (container.items[i].Id == _item.Id)
            {
                container.items[i].AddAmount(_amount);
                return;
            }
        }
        FindEmptySlot(_item, _amount);
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2)
    {
        var temp = new InventorySlot(item1.Id, item1.item, item1.amount);
        item1.UpdateSlot(item2.Id, item2.item, item2.amount);
        item2.UpdateSlot(temp.Id, temp.item, temp.amount);
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < container.items.Length; i++)
        {
            if (container.items[i].Id == _item.Id)
            {
                container.items[i].UpdateSlot(-1, null, 0);
            }
        }
    }

    public void DropItem(InventorySlot _invSlot, Item _item, int _amount)
    {
        var droppedItem = Instantiate(_item.realWorldPrefab, Vector3.zero, Quaternion.identity);
        droppedItem.GetComponent<GroundItem>().item = database.items[_item.Id];
        droppedItem.GetComponent<GroundItem>().amount = _amount;
        droppedItem.GetComponent<Transform>().position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f, 2));

        _invSlot.Id = -1;
        _invSlot.item = null;
        _invSlot.amount = 0;
    }

    public InventorySlot FindEmptySlot(Item item, int amount)
    {
        for (int i = 0; i < container.items.Length; i++)
        {
            if (container.items[i].Id <= -1)
            {
                container.items[i].UpdateSlot(item.Id, item, amount);
                return container.items[i];
            }
        }
        return null;
    }

    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < container.items.Length; i++)
            {
                container.items[i].UpdateSlot(newContainer.items[i].Id, newContainer.items[i].item, newContainer.items[i].amount);
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        container = new Inventory();
    }


}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] items;
}

[System.Serializable]
public class InventorySlot
{
    public int Id = -1;
    public Item item;
    public int amount;

    public InventorySlot()
    {
        Id = -1;
        item = null;
        amount = 0;
    }
    public InventorySlot(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }

    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        Id = _id;
        item = _item;
        amount = _amount;
    }

    public void AddAmount(int value)
    {
        amount += value;
    }
}
