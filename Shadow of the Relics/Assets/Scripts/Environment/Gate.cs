using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    static Gate instance;
    
    public SpriteRenderer GateRenderer;
    public Sprite OpenGate;
    public Collider2D GateTrigger;

    public static void OnGetArtifact() => instance.TryOpenGate();

    void Awake()
    {
        instance = this;
        GateTrigger.enabled = false;
    }

    void TryOpenGate()
    {
        foreach(bool b in SaveManager.saver.CollectedArtifacts)
        {
            if(!b)
                return;
        }

        GateRenderer.sprite = OpenGate;
        GateTrigger.enabled = true;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GateTrigger.enabled = false;
        //player enter gate
    }
}
