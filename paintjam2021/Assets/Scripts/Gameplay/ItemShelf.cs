using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShelf : MonoBehaviour
{
    public Inventory Inventory;
    public List<ItemController> Items;

    private void OnValidate()
    {
        Items = new List<ItemController>(GetComponentsInChildren<ItemController>());
    }
}
