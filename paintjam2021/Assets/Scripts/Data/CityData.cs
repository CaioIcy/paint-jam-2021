using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class City
{
    public string ID;
    public Sprite BG;
}

[CreateAssetMenu]
public class CityData : ScriptableObject
{
    public List<City> Cities;
}
