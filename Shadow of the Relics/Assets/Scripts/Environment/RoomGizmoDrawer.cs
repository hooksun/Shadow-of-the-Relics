using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoomGizmoDrawer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        CameraRoom[] rooms = GameObject.FindObjectsByType<CameraRoom>(FindObjectsSortMode.None);
        foreach(CameraRoom room in rooms)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
            Gizmos.DrawCube(room.transform.position + (Vector3)room.bounds * 0.5f, (Vector3)room.bounds);
        }
    }
}
