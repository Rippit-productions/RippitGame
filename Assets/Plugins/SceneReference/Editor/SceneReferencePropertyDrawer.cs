using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var GUI = new VisualElement();
        GUI.Add(new Label($"Scene Name: {((SceneReference)property.boxedValue).SceneName}"));
        GUI.Add(new Label($"Asset Guid: {((SceneReference)property.boxedValue).AssetGUID}"));

        var  obj = (SceneReference)property.boxedValue;
        var existingValue = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(obj.AssetGUID));
        var valueField = new UnityEditor.UIElements.ObjectField("Scene Asset");
        valueField.objectType = typeof(SceneAsset);
        valueField.RegisterValueChangedCallback(callback =>
        {
            var obj = (SceneReference)property.boxedValue;
            obj.Asset = (SceneAsset)callback.newValue;
            obj.AssetGUID =  AssetDatabase.GetAssetPath(obj.Asset);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        });
        GUI.Add(valueField);
        return GUI;
    }
}
