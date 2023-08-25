using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public float OpenSpeed, carriedArtifactRadius;

    public AudioPlayer OpenMapAudio;

    static Map instance;

    Image gateIcon;
    bool closing;

    struct CarriedArtifactIcon
    {
        public Vector2 position;
        public RectTransform icon;

        public CarriedArtifactIcon(RectTransform _icon, Vector2 pos)
        {
            icon = _icon;
            position = pos;
        }
    }

    List<CarriedArtifactIcon> carriedArtifacts = new List<CarriedArtifactIcon>();

    void Awake()
    {
        instance = this;
        gateIcon = Instantiate(IconPrefab, MapBounds);
        gateIcon.sprite = GateIcon;
        SetMapPosition(gateIcon, Gate.position);
        foreach(Transform artifact in Artifacts)
        {
            Image icon = Instantiate(IconPrefab, MapBounds);
            icon.sprite = artifact.GetComponent<SpriteRenderer>().sprite;
            SetMapPosition(icon, artifact.position);
        }
        IconPrefab.transform.SetAsLastSibling();

        gameObject.SetActive(false);
    }

    public void ToggleMapInput(InputAction.CallbackContext ctx)
    {
        if(ctx.started)
            ToggleMap();
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
        instance.MapBounds.GetChild(index+1).gameObject.SetActive(false);
    }

    public static void CarryArtifact(int index) => instance.carryArtifact(index);

    void carryArtifact(int index)
    {
        index++;
        Vector2 dir = Vector2.up * carriedArtifactRadius;
        float angle = 2f * Mathf.PI / (carriedArtifacts.Count + 1);
        for(int i = 0; i < carriedArtifacts.Count; i++)
        {
            CarriedArtifactIcon icon = carriedArtifacts[i];
            icon.position = dir;
            carriedArtifacts[i] = icon;
            dir = new Vector2(dir.x * Mathf.Cos(angle) - dir.y * Mathf.Sin(angle), dir.x * Mathf.Sin(angle) + dir.y * Mathf.Cos(angle));
        }
        carriedArtifacts.Add(new CarriedArtifactIcon((RectTransform)MapBounds.GetChild(index), dir));
    }

    public static void SetGateIcon(Sprite sprite)
    {
        instance.gateIcon.sprite = sprite;
    }

    public static void ResetCarriedArtifacts()
    {
        instance.carriedArtifacts = new List<CarriedArtifactIcon>();
    }

    public static void ResetArtifactPosition(Transform artifact)
    {
        Transform icon = instance.MapBounds.GetChild(artifact.GetSiblingIndex() + 1);
        instance.SetMapPosition(icon.GetComponent<Image>(), artifact.position);
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
        foreach(CarriedArtifactIcon icon in carriedArtifacts)
        {
            icon.icon.anchoredPosition = IconPrefab.rectTransform.anchoredPosition + icon.position;
        }
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