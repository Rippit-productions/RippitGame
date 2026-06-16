using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var GUI = new VisualElement();
        SceneReference  currentvalue = (SceneReference)property.boxedValue;
        
        var valueField = new UnityEditor.UIElements.ObjectField("Scene Asset");
        valueField.objectType = typeof(SceneAsset);
        valueField.RegisterValueChangedCallback(callback =>
        {
            SceneAsset asset = (SceneAsset)callback.newValue;
            var newData = (SceneReference)property.boxedValue;
            newData.name = asset.name;
            newData.AssetGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));

            //Apply changes
            property.boxedValue = newData;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        });

        if (currentvalue.IsValid())
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(currentvalue.AssetGUID);
            SceneAsset currentAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(assetPath);
            valueField.value = currentAsset;
        }

        GUI.Add(valueField);
        return GUI;
    }
}
