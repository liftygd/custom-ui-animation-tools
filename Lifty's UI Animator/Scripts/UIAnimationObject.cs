using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIAnimationObject : ScriptableObject
{
    [SerializeReference] public UIAnimation UIAnimation;
}
