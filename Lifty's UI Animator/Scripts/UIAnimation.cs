using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class UIAnimation
{
    public RectTransform animatedObject;
    public Ease animationEase;
    public float animationDuration;
    public UIAnimationTypes animationType = UIAnimationTypes.None;
    public bool reversed;
    public bool finished;

    public IEnumerator StartAnimation(List<NodeLinkData> links = null, List<UIAnimationNodeData> nodes = null, string guid = "", UIAnimationSequence sequence = null)
    {
        CallNextAnimations(links, nodes, guid, sequence);
        Play();

        yield return new WaitForSeconds(animationDuration);
        finished = true;
    }

    protected virtual void Play()
    {
        //Animation
    }

    public void CallNextAnimations(List<NodeLinkData> links, List<UIAnimationNodeData> nodes, string guid, UIAnimationSequence sequence)
    {
        if (links.Count <= 0 || nodes.Count <= 0) return;
        
        var availableLinks = links.FindAll(x => x.PortDelayValue >= 0 && x.BaseNodeGuid == guid).ToList();

        foreach (var link in availableLinks)
        {
            var targetNode = nodes.First(x => x.Guid == link.TargetNodeGuid);
            sequence.StartAnim(targetNode, link.PortDelayValue);
        }
    }

    public static UIAnimation GetType(UIAnimationTypes animationType)
    {
        switch (animationType)
        {
            case UIAnimationTypes.None:
                return new UIAnimation();
            case UIAnimationTypes.ColorChange:
                return new UIAnimationColorChange();
            case UIAnimationTypes.MoveObjectFromOrigin:
                return new UIAnimationMoveObjectOrigin();
            case UIAnimationTypes.MoveObjectBetweenPoints:
                return new UIAnimationMoveObjectPoints();
            case UIAnimationTypes.ScaleFromPoints:
                return new UIAnimationScalePoints();
        }

        return null;
    }
}

public enum UIAnimationTypes
{
    None,
    MoveObjectFromOrigin,
    MoveObjectBetweenPoints,
    ColorChange,
    ScaleFromPoints
}
