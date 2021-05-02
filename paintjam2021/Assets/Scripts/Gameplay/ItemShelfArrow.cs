using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShelfArrow : MonoBehaviour
{
    public ItemShelf Shelf;
    private void OnValidate()
    {
        Shelf = FindObjectOfType<ItemShelf>(true);
    }

    private void OnMouseUpAsButton()
    {
        Shelf.UpPage();
    }
}
