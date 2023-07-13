using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    public static List<GrapplePoint> Points = new List<GrapplePoint>();

    public Vector2 position{get => transform.position;}

    void Start()
    {
        Points.Add(this);
    }
}
