using System.Reflection;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class UIAnimationField
{
    public static VisualElement GetFields(FieldInfo fieldInfo, UIAnimationObject obj)
    {
        return GetFieldsFunction(fieldInfo, obj);
    }
    
    private static VisualElement GetFieldsFunction(FieldInfo fieldInfo, UIAnimationObject obj)
    {
        //Rect transform
        if (fieldInfo.FieldType == typeof(RectTransform))
        {
            var newField = new ObjectField
            {
                label = fieldInfo.Name,
                objectType = typeof(RectTransform),
                allowSceneObjects = true,
                value = fieldInfo.GetValue(obj.UIAnimation) as Object
            };

            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
        
        //Ease
        if (fieldInfo.FieldType == typeof(Ease))
        {
            var newField = new EnumField(fieldInfo.Name, (Ease)fieldInfo.GetValue(obj.UIAnimation));

            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
        
        //float
        if (fieldInfo.FieldType == typeof(float))
        {
            var newField = new FloatField
            {
                label = fieldInfo.Name,
                value = (float)fieldInfo.GetValue(obj.UIAnimation)
            };
            
            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
       
        //vector2
        if(fieldInfo.FieldType == typeof(Vector2))
        {
            var newField = new Vector2Field()
            {
                label = fieldInfo.Name,
                value = (Vector2) fieldInfo.GetValue(obj.UIAnimation)
            };
            
            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
        
        //bool except the finished boolean
        if(fieldInfo.FieldType == typeof(bool) && fieldInfo.Name != "finished")
        {
            var newField = new Toggle
            {
                label = fieldInfo.Name,
                value = (bool) fieldInfo.GetValue(obj.UIAnimation)
            };
            
            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
        
        if(fieldInfo.FieldType == typeof(Color))
        {
            var newField = new ColorField()
            {
                label = fieldInfo.Name,
                value = (Color) fieldInfo.GetValue(obj.UIAnimation)
            };
            
            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }
        
        if(fieldInfo.FieldType == typeof(Component))
        {
            var newField = new ObjectField
            {
                label = fieldInfo.Name,
                objectType = typeof(Component),
                allowSceneObjects = true,
                value = fieldInfo.GetValue(obj.UIAnimation) as Object
            };
            
            newField.RegisterValueChangedCallback(etv => fieldInfo.SetValue(obj.UIAnimation, etv.newValue));
            return newField;
        }

        return null;
    }
}
