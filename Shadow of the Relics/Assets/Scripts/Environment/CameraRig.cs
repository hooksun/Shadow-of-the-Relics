using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour
{
    public Rigidbody2D Target;
    public Vector2 bounds;
    public float maxSpeed, transitionSpeed, minTransitionSpeed;

    CameraRoom current;

    CameraRoom FindTargetRoom()
    {
        foreach(CameraRoom room in CameraRoom.Rooms)
        {
            if(room.inBound(Target.transform.position))
            {
                return room;
            }
        }
        return null;
    }

    void Update()
    {
        if(current == null || !current.inBound(Target.transform.position))
        {
            current = FindTargetRoom();
        }

        Vector3 newPos = Target.transform.position + Vector3.forward * transform.position.z;
        if(current != null)
        {
            Vector2 targetUV = current.UV(Target.transform.position);
            Vector2 camLocalPosition = Vector2.Min(current.outerBounds, bounds)*0.5f + ((targetUV) * Vector2.Max(current.outerBounds-bounds, Vector2.zero));

            newPos = (Vector3)((Vector2)current.outerPosition + camLocalPosition) + Vector3.forward * transform.position.z;
        }

        Vector2 transitionDir = (newPos - transform.position);
        if(transitionDir.sqrMagnitude > Target.velocity.sqrMagnitude * Time.deltaTime)
        {
            float speed = Mathf.Max(minTransitionSpeed, transitionSpeed * Vector2.Dot(transitionDir.normalized, Target.velocity));
            transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
        }
        else
        {
            transform.position = newPos;
        }
    }
}
