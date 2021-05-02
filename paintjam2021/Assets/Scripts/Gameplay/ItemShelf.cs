using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InvLisMonoBehaviour : MonoBehaviour, IInventoryListener
{
    public Inventory Inventory;

    public abstract void OnInventoryChanged(Inventory inv);

    protected virtual void Awake()
    {
        Inventory.AddObserver(this);
    }

    protected void OnDestroy()
    {
        Inventory.RemoveObserver(this);
    }
}

public class ItemShelf : InvLisMonoBehaviour
{
    public ItemData ItemData;
    public List<ItemController> ItemCells;
    public int Page = 0;

    private void Start()
    {
        SetupItems(Inventory.Items);
    }

    public override void OnInventoryChanged(Inventory inv)
    {
        SetupItems(inv.Items);
    }

    public void UpPage()
    {
        var prevPage = Page;
        Page = (Page + 1) % Mathf.CeilToInt(Inventory.Items.Count / (float)ItemCells.Count);
        SetupItems(Inventory.Items);
        Debug.Log($"UpPage {prevPage}->{Page} | {Inventory.Items.Count / ItemCells.Count} {Inventory.Items.Count} {ItemCells.Count}");
    }

    public void SetupItems(List<Item> items)
    {
        for(var i = 0; i < ItemCells.Count; ++i)
        {
            if((i+(Page * ItemCells.Count)) < items.Count)
            {
                ItemCells[i].gameObject.SetActive(true);
                ItemCells[i].Setup(items[i + (Page * ItemCells.Count)]);
            }
            else
            {
                ItemCells[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnValidate()
    {
        ItemCells = new List<ItemController>(GetComponentsInChildren<ItemController>());
    }
}
