using UnityEngine;
using System.Collections;

/// <summary>
/// Provides 
/// </summary>
[System.Serializable]
public class SingleUnityLayer
{
    [SerializeField]
    private int mLayerIndex = 0;
    public int LayerIndex
    {
        get { return mLayerIndex; }
    }

    /// <summary>
    /// Set's the layer field to the mask value of the field number.
    /// </summary>
    /// <param name="_layerIndex"></param>
    public void Set(int _layerIndex)
    {
        if (_layerIndex > 0 && _layerIndex < 32)
            mLayerIndex = _layerIndex;
    }

    /// <summary>
    /// Returns the mask from the current layer
    /// </summary>
    public int Mask
    {
        get { return 1 << mLayerIndex; }
    }
}