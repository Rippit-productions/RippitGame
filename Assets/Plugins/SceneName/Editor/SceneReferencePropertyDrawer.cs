using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferencePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var GUI = new VisualElement();
        SceneReference  currentvalue = (SceneReference)property.boxedValue;

        var assetField = new PropertyField(property.FindPropertyRelative("sceneAsset"));
        assetField.label = property.name;

        // Update string value
        assetField.RegisterValueChangeCallback(callback =>
        {
            SceneAsset asset = (SceneAsset)callback.changedProperty.objectReferenceValue;
            if (asset != null)
            {
                SceneReference boxValue = (SceneReference)property.boxedValue;
                boxValue.sceneName = asset.name;
                property.boxedValue = boxValue;
                property.serializedObject.ApplyModifiedProperties();
            }
        });

        GUI.Add(assetField);
        return GUI;
    }
}
