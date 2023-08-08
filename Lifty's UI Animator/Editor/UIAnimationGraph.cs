using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UIAnimationGraph : EditorWindow
{
    private UIAnimationGraphView _graphView;
    private UIAnimationContainer _container;
    private ObjectField _fileField;
    private int _instanceId;
    
    [MenuItem("Tools/Lifty/UI Animation Graph")]
    public static void OpenUIAnimationGraphWindow()
    {
        var window = GetWindow<UIAnimationGraph>();
        window.titleContent = new GUIContent("Animation Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
        
        RequestDataOperation(false);
    }

    private void OnBecameVisible()
    {
        _container = EditorUtility.InstanceIDToObject(_instanceId) as UIAnimationContainer;

        if (_container == null) return;
        
        _fileField.value = _container;
    }

    private void ConstructGraphView()
    {
        _graphView = new UIAnimationGraphView(this)
        {
            name = "Animation Graph"
        };
        
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void GenerateToolbar()
    {
        var toolBar = new Toolbar();

        //File choose
        _fileField = new ObjectField
        {
            label = "File: ",
            objectType = typeof(UIAnimationContainer),
            value = _container
        };
        _fileField.RegisterValueChangedCallback(evt =>
        {
            if (evt.newValue == null) return;
            
            _container = (UIAnimationContainer) evt.newValue;
            _instanceId = _container.GetInstanceID();
        });

        toolBar.Add(_fileField);

        //Save & Load Button
        toolBar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});
        toolBar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});
        
        rootVisualElement.Add(toolBar);
    }

    private UIAnimationContainer TryFindFile(string fileName)
    {
        if (!AssetDatabase.IsValidFolder($"Assets/Resources/UI Animations/{fileName}")) return null;
        
        var file = AssetDatabase.LoadAssetAtPath<UIAnimationContainer>($"Assets/Resources/UI Animations/{fileName}/{fileName}.asset");
        return file;
    }

    private void RequestDataOperation(bool save)
    {
        if (_container == null)
        {
            //EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid file name!", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);

        if(save)
            saveUtility.SaveGraph(_container);
        else
            saveUtility.LoadGraph(_container);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
    }
}
