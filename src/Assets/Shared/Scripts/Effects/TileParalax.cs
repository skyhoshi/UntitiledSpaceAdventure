using UnityEngine;
using System.Collections;

/// <summary>
/// Paralaxed background that is a tile. Requires a Renderer and material with maintex
/// </summary>
public class TileParalax : MonoBehaviour {

    /// <summary>
    /// Object that scrolls the paralax background, usually a camera or the "player"
    /// </summary>
    [Tooltip("Object that scrolls the paralax background, usually a camera or the \"player\"")]
    public Transform    Target;

    /// <summary>
    /// Scrolling ratio
    /// </summary>
    [Tooltip("Scrolling ratio")]
    public float        Ratio = 2;

    /// <summary>
    /// Fixed follow offset
    /// </summary>
    [Tooltip("Fixed follow offset")]
    public Vector2      FixedOffset;

    private Vector2     offset = Vector2.zero;
    private Renderer    renderer;

    void Start()
    {
        FixedOffset = new Vector2(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        renderer = GetComponent<Renderer>();

        if (renderer == null)
            Debug.LogError("Missing renderer component");
    }

    /// <summary>
    /// Scroll the texture in the update
    /// </summary>
    void FixedUpdate()
    {
        if (Target == null)
            return;
        
        offset.x = -Mathf.Repeat( (Target.position.x / Ratio) + FixedOffset.x, 1);
        offset.y = -Mathf.Repeat( (Target.position.y / Ratio) + FixedOffset.y, 1);

        renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
    }
}
