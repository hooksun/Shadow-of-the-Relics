using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    static Gate instance;
    
    public SpriteRenderer GateRenderer, CrystalRenderer;
    public Image CrystalUI;
    public Sprite OpenGate;
    public Sprite[] CrystalSprites;
    public Collider2D GateTrigger;

    public static void GetArtifact(int artifacts) => instance.CollectArtifact(artifacts);

    void Awake()
    {
        instance = this;
        GateTrigger.enabled = false;
        CollectArtifact(0);
    }

    void CollectArtifact(int artifacts)
    {
        CrystalUI.sprite = CrystalRenderer.sprite = CrystalSprites[artifacts];
        if(artifacts < CrystalSprites.Length - 1)
            return;

        GateRenderer.sprite = OpenGate;
        Map.SetGateIcon(OpenGate);
        GateTrigger.enabled = true;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GateTrigger.enabled = false;
        Player.activePlayer.EnterGate();
    }
}
