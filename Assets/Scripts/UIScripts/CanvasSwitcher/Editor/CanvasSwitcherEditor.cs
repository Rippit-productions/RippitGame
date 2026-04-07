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
    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        var DefaultContainer = new IMGUIContainer(OnInspectorGUI);
        root.Add(DefaultContainer);

        root.Add(_InspectorGUI.CloneTree());

        DefaultIndexDropDown = (DropdownField)root.Q("IndexDropDown");
        return root;
    }

    private void OnEnable()
    {
        Selection.selectionChanged += () => { };
    }
}
