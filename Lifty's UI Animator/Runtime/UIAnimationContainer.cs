using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIAnimationContainer : MonoBehaviour
{
    [HideInInspector] public List<NodeLinkData> NodeLinks = new ();
    [HideInInspector] public List<UIAnimationNodeData> NodeData = new ();

    public void Clear()
    {
        NodeLinks.Clear();
        NodeData.Clear();
    }
}
