using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    public Vector2 startVelocity;
    public float scatterStartSpeed, gravity, freeFallTime, scatterTime, targettingTime, minScale;
    public bool detectPlayer;
    public AudioPlayer collectedAudio;

    public static int collectedArtifacts;

    Transform target;
    Vector2 velocity, defaultPosition;
    ArtifactState currentState = ArtifactState.uncollected;
    float time;
    bool collecting, doScale;

    static List<Artifact> carriedArtifacts;

    void Awake()
    {
        this.enabled = false;
        collectedArtifacts = 0;
        carriedArtifacts = new List<Artifact>();
        defaultPosition = transform.localPosition;

        SaveManager.OnLoad += OnLoad;
        SaveManager.OnSave += OnSave;
    }

    void OnLoad()
    {
        ArtifactState state = SaveManager.saver.Artifacts[transform.GetSiblingIndex()];
        if(state != ArtifactState.uncollected)
        {
            gameObject.SetActive(false);
            if(state == ArtifactState.carried)
            {
                carriedArtifacts.Add(this);
                Map.CarryArtifact(transform.GetSiblingIndex());
                GetComponent<Collider2D>().enabled = false;
            }
            else
            {
                collectedArtifacts++;
                Gate.GetArtifact(collectedArtifacts);
                Map.CollectArtifact(transform.GetSiblingIndex());
            }
        }
    }

    void OnSave()
    {
        if(SaveManager.saver.Artifacts == null || SaveManager.saver.Artifacts.Length != transform.parent.childCount)
        {
            SaveManager.saver.Artifacts = new ArtifactState[transform.parent.childCount];
        }
        SaveManager.saver.Artifacts[transform.GetSiblingIndex()] = currentState;
    }

    public static void ScatterCarried(Transform newTarget, bool collect)
    {
        int count = carriedArtifacts.Count;
        if(count == 0)
            return;
        Vector2 dir = Vector2.up;
        float angle = 2f * Mathf.PI / count;
        for(int i = 0; i < count; i++)
        {
            carriedArtifacts[i].Scatter(newTarget, dir, collect);
            dir = new Vector2(dir.x * Mathf.Cos(angle) - dir.y * Mathf.Sin(angle), dir.x * Mathf.Sin(angle) + dir.y * Mathf.Cos(angle));
        }
        Map.ResetCarriedArtifacts();
    }

    public void Scatter(Transform newTarget, Vector2 direction, bool collect)
    {
        target = newTarget;
        velocity = direction * scatterStartSpeed;
        collecting = collect;
        transform.localScale = Vector3.one * minScale;
        transform.localPosition = Player.activePlayer.position;
        time = -scatterTime;
        gameObject.SetActive(true);
        this.enabled = true;
        doScale = false;
        collectedAudio.Play();
    }

    public static void ResetCarried()
    {
        foreach(Artifact a in carriedArtifacts)
            a.Reset();

        carriedArtifacts = new List<Artifact>();
        Map.ResetCarriedArtifacts();
    }

    void Reset()
    {
        transform.localPosition = defaultPosition;
        transform.localScale = Vector2.one;
        currentState = ArtifactState.uncollected;

        Map.ResetArtifactPosition(transform);

        gameObject.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        this.enabled = false;
    }

    void Update()
    {
        if(time <= 0f || target == null)
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
            if(collecting)
            {
                collectedArtifacts++;
                Gate.GetArtifact(collectedArtifacts);
                carriedArtifacts.Remove(this);
                Map.CollectArtifact(transform.GetSiblingIndex());
                currentState = ArtifactState.collected;
            }
            return;
        }

        Vector2 newPos = (Vector2)transform.position + velocity * (1f - lerp) * Time.deltaTime;
        float speed = ((Vector2)(target.position - transform.position)).magnitude / (targettingTime - time);
        newPos = Vector2.MoveTowards(newPos, target.position, speed * Time.deltaTime);

        time += Time.deltaTime;

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        if(doScale)
            transform.localScale = Vector3.one * Mathf.Lerp(1f, minScale, lerp);
    }

    void OnTriggerStay2D(Collider2D player)
    {
        target = player.transform;
        GetComponent<Collider2D>().enabled = false;
        this.enabled = true;
        velocity = startVelocity;
        time = -freeFallTime;
        collectedAudio.Play();
        carriedArtifacts.Add(this);
        Map.CarryArtifact(transform.GetSiblingIndex());
        collecting = false;
        currentState = ArtifactState.carried;
        doScale = true;

        if(detectPlayer)
            Enemy.AllDetectPlayer(player.GetComponent<Player>());
    }
}
