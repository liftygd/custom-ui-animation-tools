using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationMoveObjectPoints : UIAnimation
{
    [Space(20)] 
    [SerializeField] private Vector2 xYStart;
    [SerializeField] private Vector2 xYEnd;

    protected override void Play()
    {
        if (animatedObject == null) return;
        
        var startPos = xYStart;
        var endPos = xYEnd;
        
        DOVirtual.Vector3(startPos, endPos, animationDuration, value => animatedObject.anchoredPosition = value).SetEase(animationEase);
    }
}
