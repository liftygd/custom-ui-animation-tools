using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UIAnimationColorChange : UIAnimation
{
    [Space(20)]
    public Component targetComponent;
    public Color startColor, endColor;
    public bool useBaseStartColor, useBaseEndColor;

    protected override void Play()
    {
        if (animatedObject == null) return;
        
        var target = targetComponent;

        var component = animatedObject.GetComponent(target.GetType());
        var colorField = component.GetType().GetProperty("color");
        if (colorField == null) return;
        
        var startingColor = useBaseStartColor ? (Color) colorField.GetValue(target) : startColor;
        var endingColor = useBaseEndColor ? (Color) colorField.GetValue(target) : endColor;

        if(!reversed)
            DOVirtual.Color(startingColor, endingColor, animationDuration, value => colorField.SetValue(component, value)).SetEase(animationEase);
        else
            DOVirtual.Color(endingColor, startingColor, animationDuration, value => colorField.SetValue(component, value)).SetEase(animationEase);
    }
}
