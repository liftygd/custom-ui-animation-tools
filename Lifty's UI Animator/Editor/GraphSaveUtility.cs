using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private UIAnimationGraphView _targetGraphView;
    private UIAnimationContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<UIAnimationNode> Nodes => _targetGraphView.nodes.ToList().Cast<UIAnimationNode>().ToList();
    
    public static GraphSaveUtility GetInstance(UIAnimationGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            _targetGraphView = targetGraphView
        };
    }

    public void SaveGraph(UIAnimationContainer animationContainer)
    {
        animationContainer.Clear();

        if (Edges.Any())
        {
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as UIAnimationNode;
                var inputNode = connectedPorts[i].input.node as UIAnimationNode;

                var floatSave = float.TryParse(connectedPorts[i].output.portName, out var floatParse) ? floatParse : -1f;

                animationContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    PortDelayValue = floatSave,
                    TargetNodeGuid = inputNode.GUID
                });
            }
        }

        foreach (var animationNode in Nodes.Where(node => !node.EntryPoint))
        {
            animationContainer.NodeData.Add(new UIAnimationNodeData
            {
                Guid = animationNode.GUID,
                UIAnimation = animationNode.UIAnimation.UIAnimation,
                AnimationType = animationNode.AnimationType,
                Position = animationNode.GetPosition().position
            });
        }
        
        EditorUtility.SetDirty(animationContainer);
    }
    
    public void LoadGraph(UIAnimationContainer container)
    {
        _containerCache = container;

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }

    private void ConnectNodes()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
                
                targetNode.SetPosition(new Rect(
                    _containerCache.NodeData.First(x => x.Guid == targetNodeGuid).Position,
                    _targetGraphView.DefaultNodeSize
                ));
            }
        }
    }

    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        _targetGraphView.Add(tempEdge);
    }

    private void CreateNodes()
    {
        foreach (var nodeData in _containerCache.NodeData)
        {
            var tempNode = new UIAnimationNode
            {
                title = "Animation Node",
                GUID = nodeData.Guid,
                UIAnimation = ScriptableObject.CreateInstance<UIAnimationObject>(),
                AnimationType = nodeData.AnimationType
            };
            tempNode = _targetGraphView.GenerateNode(tempNode, nodeData.Position);
            tempNode.UIAnimation.UIAnimation = nodeData.UIAnimation;
            _targetGraphView.ChangeAnimationType(tempNode, tempNode.AnimationType);

            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName));
        }
    }

    private void ClearGraph()
    {
        //First node
        if(_containerCache.NodeLinks.Count > 0)
            Nodes.Find(x => x.EntryPoint).GUID = _containerCache.NodeLinks.First(x => x.PortName == "Entry").BaseNodeGuid;

        foreach (var node in Nodes)
        {
            if(node.EntryPoint) continue;
            
            //Remove edges
            Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));
            
            _targetGraphView.RemoveElement(node);
        }
    }
}
