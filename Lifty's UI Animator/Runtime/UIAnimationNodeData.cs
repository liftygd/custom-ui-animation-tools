using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIAnimationNodeData
{
    public string Guid;
    [SerializeReference] public UIAnimation UIAnimation;
    public UIAnimationTypes AnimationType;
    public Vector2 Position;
}
