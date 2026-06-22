using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        GUI.Add(assetField);
        return GUI;
    }
}
