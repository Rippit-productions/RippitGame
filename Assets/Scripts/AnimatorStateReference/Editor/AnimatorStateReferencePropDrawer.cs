using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AnimationStateReference;

[CustomPropertyDrawer(typeof(AnimatorStateReference))]
public class AnimatorStateReferencePropDrawer : PropertyDrawer
{
    private GameObject _SelectedGameObj;
    private SerializedProperty _SerializedProperty;

    private TextField TextDisplay;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        _SerializedProperty = property;
        var root = new VisualElement();
        TextDisplay = new TextField();
        TextDisplay.isReadOnly = true;
        TextDisplay.label = $"{property.name}" ;

        root.Add(TextDisplay);

        if (Selection.activeGameObject)
        {
            _SelectedGameObj = Selection.activeGameObject;
        }

        TextDisplay.RegisterCallback<ClickEvent>(eventCallback =>
        {
            AnimatorStateReference newdata;
            newdata = AnimatorStateReferenceEditor.PopUp(_SelectedGameObj);
            
            property.boxedValue = newdata;

            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);

            _RefreshText();
        });

        _RefreshText();
        return root;
    }


    private void _RefreshText()
    {
        var value = (AnimatorStateReference)_SerializedProperty.boxedValue;
        if (!value.IsValid())
        {
            TextDisplay.value = "-";
        }
        else 
        {
            TextDisplay.value = value.GetStatePath();
        }
    }
}
