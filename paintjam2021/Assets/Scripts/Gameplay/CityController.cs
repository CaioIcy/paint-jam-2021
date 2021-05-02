using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityController : MonoBehaviour
{
    public CityData CityData;
    public SpriteRenderer Renderer;

    private void OnValidate()
    {
        //Renderer = GetComponentInChildren<SpriteRenderer>();
    }

    int idx = 0;
    public void NextCity()
    {
        if(idx < CityData.Cities.Count)
        {
            Renderer.sprite = CityData.Cities[idx++].BG;
        }
    }
}
