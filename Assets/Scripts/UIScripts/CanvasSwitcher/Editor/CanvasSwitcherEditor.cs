using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CanvasSwitcher))]
[InitializeOnLoad]
public class CanvasSwitcherEditor : Editor
{
    public VisualTreeAsset _InspectorGUI;
    public DropdownField DefaultIndexDropDown;
    CanvasSwitcher _Component;

    static CanvasSwitcherEditor()
    {
        UnityEditor.Selection.selectionChanged += RefreshAll;
        EditorApplication.hierarchyChanged += CheckValues;
    }

    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        DefaultIndexDropDown = new DropdownField();
        DefaultIndexDropDown.label = "Default Layer";
        root.Add(DefaultIndexDropDown);

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
            RefreshAll();
        });
        return root;
    }

    private void OnEnable()
    {
        _Component = (CanvasSwitcher)target;
        Selection.selectionChanged += () => { };

        
    }

    /// <summary>
    /// Refresh All Canvas Switcher in current scene
    /// </summary>
    private static void RefreshAll()
    {
        var AllComponents = FindObjectsByType<CanvasSwitcher>(FindObjectsSortMode.InstanceID);
        if (AllComponents.Length == 0) return;
        GameObject selected = UnityEditor.Selection.activeGameObject;
        foreach (var Component in AllComponents)
        {
            if (selected != null 
                && selected.transform.IsChildOf(Component.transform)
                && selected.transform != Component.transform)
            {
                for (int i = 0; i < Component.transform.childCount; i++)
                {
                    if (selected.transform.IsChildOf(Component.transform.GetChild(i)) ||
                        selected.transform == Component.transform.GetChild(i))
                    {
                        Component.SetActiveIndex(i);
                    }
                }
            }
            else
            {
                Component.SetActiveIndex(Component.DefaultIndex);
            }
        }
    }

    private static void CheckValues()
    {
        var AllComponents = FindObjectsByType<CanvasSwitcher>(FindObjectsSortMode.InstanceID);
        if (AllComponents.Length == 0) return;

        foreach (var Component in AllComponents)
        {
            int maxValue = Component.transform.childCount - 1;
            if (Component.DefaultIndex >= maxValue)
            {
                Component.DefaultIndex = maxValue;
            }
        }
    }
}
