using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationScalePoints : UIAnimation
{
    [Space(20)] 
    [SerializeField] private float startScale;
    [SerializeField] private float endScale;

    protected override void Play()
    {
        if (animatedObject == null) return;

        DOVirtual.Float(startScale, endScale, animationDuration, value => animatedObject.localScale = value * Vector3.one).SetEase(animationEase);
    }
}
