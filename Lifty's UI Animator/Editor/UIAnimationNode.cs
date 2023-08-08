using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UIAnimationNode : Node
{
    public string GUID;

    public UIAnimationTypes AnimationType = UIAnimationTypes.None;
    public UIAnimationObject UIAnimation;

    public bool EntryPoint = false;
}
