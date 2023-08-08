using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UIAnimationContainer))]
public class UIAnimationSequence : MonoBehaviour
{
    [Header("Invoke states")] 
    [SerializeField] private bool invokeOnStart; 
    [SerializeField] private bool invokeOnEnd;
    [Space(15)]
    
    [Header("Events")]
    public UnityEvent AnimationStartAction;
    public UnityEvent AnimationFinishAction;
    [Space(15)]
    
    [Header("Animation")] 
    [SerializeField] private UIAnimationContainer animationToPlay;

    private void OnEnable()
    {
        if(invokeOnStart)
            PlayAnimation();
    }

    private void OnDisable()
    {
        if(invokeOnEnd)
            PlayAnimation();
    }

    public void PlayAnimation()
    {
        animationToPlay = GetComponent<UIAnimationContainer>();
        
        if (animationToPlay == null) return;
        
        AnimationStartAction?.Invoke();
        
        animationToPlay.NodeData.ForEach(x => x.UIAnimation.finished = false);
        StartCoroutine(Sequence());
    }

    private IEnumerator Sequence()
    {
        //Play first
        var entryLink = animationToPlay.NodeLinks.First(x => x.PortName == "Entry");
        var firstAnim = animationToPlay.NodeData.First(x => x.Guid == entryLink.TargetNodeGuid);
        StartAnim(firstAnim, 0f);

        while (animationToPlay.NodeData.Any(x => !x.UIAnimation.finished))
        {
            yield return null;
        }
        
        AnimationFinishAction?.Invoke();
    }

    public void StartAnim(UIAnimationNodeData node, float delay)
    {
        StartCoroutine(StartAnimDelay(node, delay));
    }

    private IEnumerator StartAnimDelay(UIAnimationNodeData node, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        StartCoroutine(node.UIAnimation.StartAnimation(animationToPlay.NodeLinks, animationToPlay.NodeData, node.Guid, this));
    }
}
