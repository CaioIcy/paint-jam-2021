using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienController : MonoBehaviour
{
    public SpriteRenderer Renderer;

    public void Setup(Alien alien)
    {
        Renderer.sprite = alien.Sprite;
    }

    private void OnValidate()
    {
        Renderer = GetComponentInChildren<SpriteRenderer>();
    }
}
