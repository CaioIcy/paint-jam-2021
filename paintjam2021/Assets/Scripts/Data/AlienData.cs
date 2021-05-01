using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Alien
{
    public string Name;
    public Sprite Sprite;
}

public class AlienData : ScriptableObject
{
    public List<Alien> Aliens;
}
