using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public Vector2 startVelocity;
    public float gravity, freeFallTime, targettingTime, minScale;
    public bool detectPlayer;

    public static int collectedArtifacts;

    Transform target;
    Vector2 velocity;
    float time;

    void Awake()
    {
        this.enabled = false;
        collectedArtifacts = 0;
    }

    void Update()
    {
        if(time <= 0f)
        {
            transform.position += (Vector3)velocity * Time.deltaTime;
            velocity.y -= gravity * Time.deltaTime;
            time += Time.deltaTime;
            return;
        }

        float lerp = Mathf.InverseLerp(0f, targettingTime, time);

        if(lerp >= 1f)
        {
            gameObject.SetActive(false);
            return;
        }

        Vector2 newPos = (Vector2)transform.position + velocity * (1f - lerp) * Time.deltaTime;
        float speed = ((Vector2)(target.position - transform.position)).magnitude / (targettingTime - time);
        newPos = Vector2.MoveTowards(newPos, target.position, speed * Time.deltaTime);

        time += Time.deltaTime;

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        transform.localScale = Vector3.one * Mathf.Lerp(1f, minScale, lerp);
    }

    void OnTriggerStay2D(Collider2D player)
    {
        target = player.transform;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = true;
        velocity = startVelocity;
        time = -freeFallTime;
        collectedArtifacts++;

        if(detectPlayer)
            Enemy.AllDetectPlayer(player.GetComponent<Player>());
    }
}
