using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private UIAnimationGraphView _graphView;
    private EditorWindow _window;
    private Texture2D _indentationIcon;

    public void Init(UIAnimationGraphView graphView, EditorWindow window)
    {
        _graphView = graphView;
        _window = window;

        //For search window fake padding
        _indentationIcon = new Texture2D(1, 1);
        _indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
        _indentationIcon.Apply();
    }
    
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Animation Node"), 1),
            new SearchTreeEntry(new GUIContent("Basic Node", _indentationIcon))
            {
                userData = new UIAnimationNode(), level = 2
            }
        };

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        var worldMousePos = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
            context.screenMousePosition - _window.position.position);
        var localMousePos = _graphView.contentViewContainer.WorldToLocal(worldMousePos);
        
        switch (SearchTreeEntry.userData)
        {
            case UIAnimationNode animationNode:
                _graphView.CreateNode("Animation Node", localMousePos);
                return true;
            
            default:
                return false;
        }
    }
}
