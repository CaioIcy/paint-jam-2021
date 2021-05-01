using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public string Name;
    public Sprite Sprite;
}

public class ItemData : ScriptableObject
{
    public List<Item> Items;
}
