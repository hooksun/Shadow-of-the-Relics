using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Image IconPrefab;
    public Sprite PlayerIcon, GateIcon;
    public Transform Artifacts, Gate;
    public Tilemap Level;
    public RectTransform rectTransform, MapBounds;
    public Vector2 LevelBoundsStart, LevelBoundsEnd;
    public float OpenSpeed;

    public AudioPlayer OpenMapAudio;

    static Map instance;

    Image gateIcon;
    bool closing;

    void Awake()
    {
        instance = this;
        foreach(Transform artifact in Artifacts)
        {
            Image icon = Instantiate(IconPrefab, MapBounds);
            icon.sprite = artifact.GetComponent<SpriteRenderer>().sprite;
            SetMapPosition(icon, artifact.position);
        }
        gateIcon = Instantiate(IconPrefab, MapBounds);
        gateIcon.sprite = GateIcon;
        SetMapPosition(gateIcon, Gate.position);
        IconPrefab.transform.SetAsLastSibling();

        gameObject.SetActive(false);
    }

    public void ToggleMap()
    {
        if(!gameObject.activeInHierarchy)
        {
            rectTransform.anchoredPosition = Vector2.down * Screen.height;
            gameObject.SetActive(true);
            closing = false;
        }
        else
            closing = !closing;
        
        OpenMapAudio.Play();
    }

    public static void CollectArtifact(int index)
    {
        instance.MapBounds.GetChild(index).gameObject.SetActive(false);
    }

    public static void SetGateIcon(Sprite sprite)
    {
        instance.gateIcon.sprite = sprite;
    }

    void Update()
    {
        Vector2 targetPos = (closing?Vector2.down*Screen.height:Vector2.zero);
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPos, OpenSpeed * Screen.height * Time.deltaTime);
        if(closing && rectTransform.anchoredPosition == targetPos)
        {
            gameObject.SetActive(false);
            return;
        }

        SetMapPosition(IconPrefab, Player.activePlayer.position);
    }

    void SetMapPosition(Image icon, Vector2 worldPos)
    {
        Vector2 screenPos = worldPos - LevelBoundsStart;
        screenPos /= (LevelBoundsEnd - LevelBoundsStart);

        screenPos -= Vector2.one * 0.5f;

        screenPos *= MapBounds.rect.size;
        icon.rectTransform.anchoredPosition = screenPos;
    }
}