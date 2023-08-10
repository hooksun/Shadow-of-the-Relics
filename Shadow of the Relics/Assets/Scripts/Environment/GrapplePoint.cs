using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public static List<GrapplePoint> Points = new List<GrapplePoint>();

    public Vector2 position{get => transform.position;}
    public SpriteRenderer Renderer;
    public Sprite[] sprites;

    void OnEnable()
    {
        Points.Add(this);
    }

    void OnDisable()
    {
        Points.Remove(this);
    }

    public void SetSprite(int index)
    {
        Renderer.sprite = sprites[index];
    }
}
