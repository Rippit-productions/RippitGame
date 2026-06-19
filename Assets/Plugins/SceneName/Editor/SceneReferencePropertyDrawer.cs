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

        assetField.RegisterValueChangeCallback(value => {
            SceneAsset sceneasset = (SceneAsset)value.changedProperty.boxedValue;
            SceneReference newData = (SceneReference)property.boxedValue;
            if (sceneasset == null) 
            {
                newData.name = null;

            }

            newData.name = sceneasset.name;
            property.boxedValue = newData;
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            property.serializedObject.ApplyModifiedProperties();
        });

        GUI.Add(assetField);
        return GUI;
    }
}
