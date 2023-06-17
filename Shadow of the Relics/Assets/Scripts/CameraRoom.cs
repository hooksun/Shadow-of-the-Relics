using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoom : MonoBehaviour
{
    public static List<CameraRoom> Rooms = new List<CameraRoom>();

    public Vector2 bounds;

    void Start()
    {
        Rooms.Add(this);
    }

    public Vector2 UV(Vector2 pos)
    {
        pos -= (Vector2)transform.position;

        pos.x /= bounds.x;
        pos.y /= bounds.y;
        return pos;
    }

    public bool inBound(Vector2 pos)
    {
        pos = UV(pos);
        return (Vector2.Max(pos, Vector2.one) == Vector2.one && Vector2.Min(pos, Vector2.zero) == Vector2.zero);
    }
}
