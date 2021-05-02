using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class God : MonoBehaviour
{
    public bool ForceOne = false;

    public Inventory Inventory;
    public AlienData AlienData;
    public ItemData ItemData;


    public static God ins;

    public AlienController AlienCt;
    public CityController CityCt;
    public List<string> PrevAliens = new List<string>();
    public UIGameplay _ui;

    private void OnValidate()
    {
        AlienCt = FindObjectOfType<AlienController>(true);
        CityCt = FindObjectOfType<CityController>(true);
        _ui = FindObjectOfType<UIGameplay>(true);
    }

    private void Awake()
    {
        ins = this;
        var initialItems = ItemData.GetInitialItems();
        Inventory.AddItems(initialItems);

        CheckNext();
    }

    int cityIdx = 0;
    public void CheckNext()
    {
        var drawnAlien = DrawAlien();
        if(drawnAlien == null)
        {
            _ui.SetupEnding();
            return;
        }
        AlienCt.Setup(AlienData.GetAlienByID(drawnAlien));
        AlienCt.EnterScene();
        PrevAliens.Add(drawnAlien);

        if (cityIdx > AlienData.Aliens.Count / CityCt.CityData.Cities.Count)
        {
            CityCt.NextCity();
            cityIdx = 0;
        }
        cityIdx++;
    }

    string DrawAlien()
    {
        var aliens = AlienData.Aliens.Where(a => !PrevAliens.Contains(a.ID)).Select(a => a.ID).ToList();
        if(aliens.Count <= 0 || (ForceOne && PrevAliens.Count >= 1))
        {
            return null;
        }
        var drawn = aliens[UnityEngine.Random.Range(0, aliens.Count)];
        return drawn;
    }
}
