using AnimationStateReference;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimatorStateReferencePropDrawerGUI : ScriptableObject
{
    [SerializeField] private VisualTreeAsset _GUIAsset;
    public VisualElement GUI => _GUI;
    private VisualElement _GUI;

    private Label _ProperptyLabel;
    private TextField _StateNameTextField;
    private Button _SearchButton;

    private SerializedProperty _Property;
    private void Awake()
    {
        _GUI = _GUIAsset.CloneTree();
        _ProperptyLabel = _GUI.Q<Label>("PropertyLabel");
        _StateNameTextField = _GUI.Q<TextField>("StateName");
        _SearchButton = _GUI.Q<Button>("SearchButton");

        _SearchButton.clicked += () =>
        {
            AnimatorStateReference newdata;
            newdata = AnimatorStateReferenceEditor.PopUp(Selection.activeGameObject);

            if (newdata && _Property != null)
            {
                Undo.RecordObject(_Property.serializedObject.targetObject, "Animatior State Reference Change");
                _Property.boxedValue = newdata;
                _Property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_Property.serializedObject.targetObject);
                Refresh();
            }
        };

        _StateNameTextField.doubleClickSelectsWord = false;
        _StateNameTextField.tripleClickSelectsLine = false;

        _StateNameTextField.RegisterCallback<ClickEvent>(callback =>
        {
            AnimatorStateReference newdata;
            newdata = AnimatorStateReferenceEditor.PopUp(Selection.activeGameObject);

            if (newdata && _Property != null)
            {
                Undo.RecordObject(_Property.serializedObject.targetObject, "Animatior State Reference Change");
                _Property.boxedValue = newdata;
                _Property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(_Property.serializedObject.targetObject);
                Refresh();
            }
        });
    }

    public void SetProperty(SerializedProperty property)
    {
        _Property = property;
        Refresh();
    }

    public void Refresh()
    {
        if (_Property == null) return;
        _ProperptyLabel.text = _Property.name; 

        if (_Property.boxedValue != null)
        {
            var property = (AnimatorStateReference)_Property.boxedValue;
            _StateNameTextField.value = property;
            _StateNameTextField.isReadOnly = true;
        }
    }
}
