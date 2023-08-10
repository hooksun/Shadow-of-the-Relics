using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CameraRoom : MonoBehaviour
{
    public static List<CameraRoom> Rooms = new List<CameraRoom>();

    public Vector2Int bounds, TRborder, BLborder;

    public Vector2 outerBounds{get=>bounds + BLborder + TRborder;}
    public Vector2 boundPosition{get=>transform.position;}
    public Vector2 outerPosition{get=>boundPosition - BLborder;}

    void OnEnable()
    {
        Rooms.Add(this);
    }

    void OnDisable()
    {
        Rooms.Remove(this);
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
        pos -= (Vector2)transform.position;
        return (Vector2.Max(pos, bounds) == bounds && Vector2.Min(pos, Vector2.zero) == Vector2.zero);
    }

    public bool inOuterBound(Vector2 pos)
    {
        pos -= outerPosition;
        return (Vector2.Max(pos, outerBounds) == outerBounds && Vector2.Min(pos, Vector2.zero) == Vector2.zero);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.05f);
        Gizmos.DrawCube(outerPosition + outerBounds * 0.5f, outerBounds);
        Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
        Gizmos.DrawCube(boundPosition + (Vector2)bounds * 0.5f, (Vector2)bounds);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        Gizmos.DrawCube(outerPosition + outerBounds * 0.5f, outerBounds);
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
        Gizmos.DrawCube(boundPosition + (Vector2)bounds * 0.5f, (Vector2)bounds);
    }
#endif
}
