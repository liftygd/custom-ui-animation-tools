using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAnimationGraphView : GraphView
{
    public readonly Vector2 DefaultNodeSize = new Vector2(150, 200);
    private NodeSearchWindow _searchWindow;
    
    public UIAnimationGraphView(EditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("AnimationGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var gridBg = new GridBackground();
        Insert(0, gridBg);
        gridBg.StretchToParentSize();
        
        AddElement(GenerateEntryPointNode());
        AddSearchWindow(editorWindow);
    }

    private void AddSearchWindow(EditorWindow editorWindow)
    {
        _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
        _searchWindow.Init(this, editorWindow);
        
        nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
    }

    #region Port Generation
    
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port) =>
        {
            if(startPort != port && startPort.node != port.node && startPort.direction != port.direction)
               compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private Port GeneratePort(UIAnimationNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    
    public void AddChoicePort(UIAnimationNode animationNode, string overridenPortName = "")
    {
        var generatedPort = GeneratePort(animationNode, Direction.Output);

        var outputPortCount = animationNode.outputContainer.Query("connector").ToList().Count;

        var choicePortName = string.IsNullOrEmpty(overridenPortName) ? "0" : overridenPortName;
        generatedPort.portName = choicePortName;

        var floatLoad = float.TryParse(generatedPort.portName, out var floatParse) ? floatParse : 0f;
        //Animation delay time exit
        var floatField = new FloatField
        {
            name = string.Empty
        };
        floatField.layout.Set(0, 0, 0, 0);
        floatField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue.ToString(CultureInfo.CurrentCulture));
        floatField.value = floatLoad;
        
        var label = new Label("  ");
        label.contentContainer.Add(floatField);
        generatedPort.contentContainer.Add(label);
        
        //Delete button
        var deleteButton = new Button(() => RemovePort(animationNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        
        animationNode.outputContainer.Add(generatedPort);
        animationNode.RefreshExpandedState();
        animationNode.RefreshPorts();
    }

    private void RemovePort(UIAnimationNode animationNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);

        if (!targetEdge.Any()) return;

        var edge = targetEdge.First();
        edge.input.Disconnect(edge);
        RemoveElement(targetEdge.First());
        
        animationNode.outputContainer.Remove(generatedPort);
        animationNode.RefreshExpandedState();
        animationNode.RefreshPorts();
    }
    
    #endregion

    #region Node Generation
    private UIAnimationNode GenerateEntryPointNode()
    {
        var node = new UIAnimationNode
        {
            title = "START",
            UIAnimation = ScriptableObject.CreateInstance<UIAnimationObject>(),
            GUID = Guid.NewGuid().ToString(),
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Entry";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;
        node.capabilities &= ~Capabilities.Copiable;
        
        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(100, 200, 100, 150));
        return node;
    }
    
    public void CreateNode(string nodeName, Vector2 position)
    {
        AddElement(CreateAnimationNode(nodeName, position));
    }

    public UIAnimationNode CreateAnimationNode(string nodeName, Vector2 position)
    {
        var node = new UIAnimationNode
        {
            title = nodeName,
            UIAnimation = ScriptableObject.CreateInstance<UIAnimationObject>(),
            GUID = Guid.NewGuid().ToString()
        };
        
        return GenerateNode(node, position);
    }

    public UIAnimationNode GenerateNode(UIAnimationNode node, Vector2 position)
    {
        var inputPort = GeneratePort(node, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Start";
        node.inputContainer.Add(inputPort);

        var button = new Button(() => { AddChoicePort(node); });
        button.text = "+";
        node.titleContainer.Add(button);
        
        //Node type choice
        var enumField = new EnumField("", node.AnimationType);
        enumField.RegisterValueChangedCallback(etv =>
        {
            ChangeAnimationType(node, (UIAnimationTypes) enumField.value);
            node.AnimationType = (UIAnimationTypes) enumField.value;
        });
        node.mainContainer.Add(enumField);
        
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        node.SetPosition(new Rect(position, DefaultNodeSize));

        return node;
    }

    //Custom ui animation property
    public void ChangeAnimationType(UIAnimationNode node, UIAnimationTypes animationType)
    {
        node.extensionContainer.Clear();
        node.RefreshExpandedState();

        if (animationType == UIAnimationTypes.None) return;

        if(node.AnimationType != animationType)
            node.UIAnimation.UIAnimation = UIAnimation.GetType(animationType);
        
        var animPropertyObj = new SerializedObject(node.UIAnimation);
        GetProperties(animPropertyObj.FindProperty("UIAnimation"), node);
        node.RefreshExpandedState();
    }

    private void GetProperties(SerializedProperty property, UIAnimationNode node)
    {
        var anim = property;

        var animType = anim.type.Replace("managedReference<", "").Replace(">", "");
        var type = Type.GetType(animType + ", Assembly-CSharp-firstpass, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
        var variables = type?.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        
        if (variables == null) return;
        
        foreach (var field in variables)
        {
            node.extensionContainer.Add(UIAnimationField.GetFields(field, node.UIAnimation));
        }
    }
    
    #endregion
}
