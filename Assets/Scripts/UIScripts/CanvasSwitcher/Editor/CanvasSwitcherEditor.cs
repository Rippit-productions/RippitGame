using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CanvasSwitcher))]
public class CanvasSwitcherEditor : Editor
{
    public VisualTreeAsset _InspectorGUI;
    public DropdownField DefaultIndexDropDown;

    CanvasSwitcher _Component;
    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        DefaultIndexDropDown = new DropdownField();
        DefaultIndexDropDown.label = "Default Layer";
        root.Add(DefaultIndexDropDown);
        var DefaultContainer = new IMGUIContainer(OnInspectorGUI);
        root.Add(DefaultContainer);


        foreach (Transform t in _Component.transform)
        {
            DefaultIndexDropDown.choices.Add(t.gameObject.name);
        }
        int currentDefault = _Component.DefaultIndex;
        DefaultIndexDropDown.value = _Component.transform.GetChild(currentDefault).gameObject.name;

        DefaultIndexDropDown.RegisterValueChangedCallback(e =>
        {
            int newIndex = _Component.transform.Find(e.newValue).GetSiblingIndex();
            _Component.DefaultIndex = newIndex;
        });
        return root;
    }

    private void OnEnable()
    {
        _Component = (CanvasSwitcher)target;
        Selection.selectionChanged += () => { };
    }
}
