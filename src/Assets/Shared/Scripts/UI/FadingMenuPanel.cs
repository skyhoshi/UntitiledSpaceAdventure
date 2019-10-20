using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Creates a menu that fades away after a specified time (The fade time is not configurable [yet!])
/// </summary>
public class FadingMenuPanel : MonoBehaviour {

    private CanvasRenderer Renderer;

    private bool mInitialized = false;

    private CanvasRenderer[] CanvasList;

    float fadeTime = 5;

    void Validate()
    {
        bool valid = true;

        Renderer = gameObject.GetComponent<CanvasRenderer>();
        if (Renderer == null)
        {
            Debug.LogError("No canvas renderer attached");
            valid = false;
        }

        // Get all the canvases to apply the change to
        CanvasList = gameObject.GetComponentsInChildren<CanvasRenderer>();
        
        if (valid == true)
            mInitialized = true;
    }

    void OnEnable()
    {
        Validate();
    }

    void Update()
    {
        // Fades down over 1 second once the count down reaches 1 second remianing
        fadeTime -= Time.fixedDeltaTime;
        float alpha = Mathf.Max(Mathf.Min(Renderer.GetAlpha(), fadeTime), 0);

        // Apply fade to all interface componentss
        for (int i = 0; i < CanvasList.Length; i++)
            CanvasList[i].SetAlpha(alpha);
    }

}
