using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BaseColorSetting : MonoBehaviour {

    [Header("Element0: False ::: Element1: True")]
    public Color[] baseColor = new Color[2];
    public Color[] lineColor01 = new Color[2];
    public Color[] lineColor02 = new Color[2];
    public Color[] iconColor = new Color[2];
    public Color[] textColor = new Color[2];

    public Color[] GetBaseColor()
    {
        return baseColor;
    }

    public Color[] GetLineColor01()
    {
        return lineColor01;
    }

    public Color[] GetLineColor02()
    {
        return lineColor02;
    }

    public Color[] GetIconColor()
    {
        return iconColor;
    }
    public Color[] GetTextColor()
    {
        return textColor;
    }


}
