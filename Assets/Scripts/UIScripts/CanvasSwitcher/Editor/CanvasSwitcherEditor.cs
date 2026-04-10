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

        var selected = Selection.activeGameObject.transform;
        for (int i = 0; i < Target.transform.childCount; i++)
        {
            var indexTransform = Target.transform.GetChild(i); 
            if (
                selected == indexTransform || 
                selected.IsChildOf(indexTransform))
            {
                SceneVisibilityManager.instance.Show(indexTransform.gameObject, true);
            }
            else
            {
                SceneVisibilityManager.instance.Hide(indexTransform.gameObject, true);
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
