using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(CanvasSwitcher))]
[InitializeOnLoad]
public class CanvasSwitcherEditor : Editor
{
    public VisualTreeAsset _InspectorGUI;
    public DropdownField DefaultIndexDropDown;
    CanvasSwitcher _Component;


    private static bool _InPrefabMode;
    private static PrefabStage _PrefabStage;

    static CanvasSwitcherEditor()
    {
        Selection.selectionChanged += _EditorRefreshAll;
        EditorApplication.hierarchyChanged += CheckValues;
        PrefabStage.prefabStageOpened += (stage) =>
        {
            _InPrefabMode = true;
            _PrefabStage = stage;
        };
        PrefabStage.prefabStageClosing += (stage) =>
        {
            _InPrefabMode = false;
            _PrefabStage = null;
        };
    }

    public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        DefaultIndexDropDown = new DropdownField();
        DefaultIndexDropDown.label = "Default Layer";
        root.Add(DefaultIndexDropDown);


        if (_Component.transform.childCount > 0)
        {
            foreach (Transform t in _Component.transform)
            {
                DefaultIndexDropDown.choices.Add(t.gameObject.name);
            }

            int currentDefault = _Component.DefaultIndex;
            DefaultIndexDropDown.value = _Component.transform.GetChild(currentDefault).gameObject.name;
        }
        else
        {
            DefaultIndexDropDown.value = "----";
            DefaultIndexDropDown.SetEnabled(false);
        }

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

    private static void _EditorRefresh(CanvasSwitcher Target)
    {
        if (Target == null) return;
        bool ShowDefault = true;

        if (Selection.activeTransform)
        {
            if (Selection.activeTransform.IsChildOf(Target.transform) && Selection.activeTransform != Target.transform)
            {
                ShowDefault = false;
            }
        }

        for (int i = 0; i < Target.transform.childCount; i++)
        {
            GameObject indexObj = Target.transform.GetChild(i).gameObject;
            Transform indexTransform = Target.transform.GetChild(i);
            if (ShowDefault)
            {
                SceneVisibilityManager.instance.Hide(indexObj, true);
                if (i == Target.DefaultIndex)
                {
                    SceneVisibilityManager.instance.Show(indexObj, true);
                }
            }
            else
            {
                SceneVisibilityManager.instance.Hide(indexObj, true);
                if (Selection.activeTransform == indexTransform ||
                    Selection.activeTransform.IsChildOf(indexTransform)){
                    SceneVisibilityManager.instance.Show(indexObj, true);
                }
            }
        }
    }

    private static void _EditorRefreshAll()
    {
        GameObject[] rootObjs;

        if (_InPrefabMode)
        {
            rootObjs = _PrefabStage.scene.GetRootGameObjects();
        }
        else {
            rootObjs = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        }


        foreach (var obj in rootObjs)
        {
            CanvasSwitcher[] components = obj.GetComponentsInChildren<CanvasSwitcher>();
            foreach (var component in components)
            {
                _EditorRefresh(component);
            }
        }
    }

}
