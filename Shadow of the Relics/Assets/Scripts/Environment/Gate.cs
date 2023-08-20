using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    static Gate instance;
    
    public SpriteRenderer GateRenderer, CrystalRenderer;
    public Image CrystalUI;
    public Sprite OpenGate;
    public Sprite[] CrystalSprites;
    public Collider2D GateTrigger;

    public UnityEvent OnOpenGate, OnPlayerEnter;

    public static void GetArtifact(int artifacts) => instance.CollectArtifact(artifacts);

    bool gateIsOpen;

    void Awake()
    {
        instance = this;
        CollectArtifact(0);
    }

    void CollectArtifact(int artifacts)
    {
        CrystalUI.sprite = CrystalRenderer.sprite = CrystalSprites[artifacts];
        if(artifacts < CrystalSprites.Length - 1)
            return;

        GateRenderer.sprite = OpenGate;
        gateIsOpen = true;
        Map.SetGateIcon(OpenGate);
        OnOpenGate.Invoke();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Artifact.ScatterCarried(CrystalRenderer.transform, true);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(!gateIsOpen)
            return;

        GateTrigger.enabled = false;
        Player.activePlayer.EnterGate();
        OnPlayerEnter.Invoke();
    }
}
