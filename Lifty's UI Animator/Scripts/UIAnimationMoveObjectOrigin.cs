using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIAnimationMoveObjectOrigin : UIAnimation
{
    [Space(20)] 
    [SerializeField] private Vector2 xYMove;

    protected override void Play()
    {
        if (animatedObject == null) return;
        
        var startPos = animatedObject.anchoredPosition;
        
        var endPos = startPos + (reversed ? -xYMove : xYMove);

        DOVirtual.Vector3(startPos, endPos, animationDuration, value => animatedObject.anchoredPosition = value).SetEase(animationEase);
    }
}
