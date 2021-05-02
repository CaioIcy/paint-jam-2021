using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public interface IInventoryListener
{
    void OnInventoryChanged(Inventory inv);
}

public class Inventory : MonoBehaviour
{
    public int Money;
    public List<Item> Items;

    public HashSet<IInventoryListener> Observers = new HashSet<IInventoryListener>();
    public void AddObserver(IInventoryListener obs)
    {
        Observers.Add(obs);
    }
    public void RemoveObserver(IInventoryListener obs)
    {
        Observers.Remove(obs);
    }
    public void Notify()
    {
        foreach(var o in Observers)
        {
            o.OnInventoryChanged(this);
        }
    }

    public void DeltaMoney(int delta)
    {
        Money += delta;
        Notify();
    }

    public void AddItems(List<Item> items)
    {
        foreach(var item in items)
        {
            if(HasItem(item))
            {
                Debug.LogError($"cant have same item twice [{item.Name}]");
                continue;
            }
            Items.Add(item);
        }
        Notify();
    }
    public void AddItem(Item item) { AddItems(new List<Item> { item }); }
    public void RemoveItem(string itemID)
    {
        var item = GetItem(itemID);
        Items.Remove(item);
        Notify();
    }

    public Item GetItem(string itemID)
    {
        return Items.SingleOrDefault(it => it.ID == itemID);
    }

    public bool HasItem(Item item)
    {
        return HasItem(item.ID);
    }

    public bool HasItem(string itemID)
    {
        return GetItem(itemID) != null;
    }
}
