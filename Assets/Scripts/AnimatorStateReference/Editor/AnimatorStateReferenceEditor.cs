using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomEditor(typeof(AnimatorStateReference))]
public class AnimatorStateReferenceEditor : Editor
{
    public VisualTreeAsset _EditorGUI;

    private AnimatorStateReference _Asset;


    private ObjectField ControllerField;
    private TextField StateNameField;
    private TreeView StateNamesView;

    private Action OnAnyChange = () => { };

    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        var GUI = _EditorGUI.CloneTree();

        root.Add(GUI);

        ControllerField = GUI.Q<ObjectField>("ControllerObjectField");
        StateNameField = GUI.Q<TextField>("StateNameField");
        StateNamesView = GUI.Q<TreeView>("StatesTreeView");

        _Asset = (AnimatorStateReference)target;
        OnAnyChange += () =>
        {
            EditorUtility.SetDirty(_Asset);
        };

        if (_Asset.Controller != null)
        {
            ControllerField.value = _Asset.Controller;
        }
        ControllerField.RegisterValueChangedCallback(change => {
            _Asset.Controller = (AnimatorController)change.newValue;
            _RefreshOptionList();
            OnAnyChange.Invoke();
        });

        StateNameField.value = _Asset.StateName.GetFullStateName();


        StateNamesView.makeItem = () => new Label();
        StateNamesView.bindItem = (element, id) =>
        {
            var data = StateNamesView.GetItemDataForIndex<(bool IsLayer, AnimatorStateName StateString)>(id);
            if (data.IsLayer) 
            {
                ((Label)element).text =  data.StateString.LayerName;
            }
            else
            {
                ((Label)element).text = StateNamesView.GetItemDataForIndex<(bool IsLayer, AnimatorStateName StateString)>(id).StateString.GetFullStateName();
            }
        };

        StateNamesView.selectedIndicesChanged += (selectIndex) =>
        {
            _Asset.StateName = StateNamesView.GetItemDataForIndex<(bool IsLayer, AnimatorStateName StateString)>(selectIndex.First()).StateString;
            StateNameField.value = _Asset.StateName.GetFullStateName();
            OnAnyChange.Invoke();
        };

        
        _RefreshOptionList();
        return root;
        
    }

    private void _RefreshOptionList()
    {
        if (ControllerField.value == null) return;
        AnimatorController controller = ControllerField.value as AnimatorController;

        List<TreeViewItemData<(bool IsLayer,AnimatorStateName StateString)>> Items = new List<TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)>>();
        foreach (var layer in controller.layers) 
        {
            List<TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)>> childItems = new List<TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)>>();
            foreach (var child in layer.stateMachine.states)
            {
                int childGuid = Guid.NewGuid().GetHashCode();
                TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)> childData = new TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)>(
                    childGuid,
                    (false, new AnimatorStateName()
                    {
                        LayerName = layer.name,
                        StateName = child.state.name
                    }
                    ));
                childItems.Add(childData);
            }
            int LayerGUID = Guid.NewGuid().GetHashCode();
            AnimatorStateName LayerData = new AnimatorStateName
            {
                LayerName = layer.name,
                StateName = null,
            };
            TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)> item = new TreeViewItemData<(bool IsLayer, AnimatorStateName StateString)>(LayerGUID,(true,LayerData),childItems);
            Items.Add(item);
        }

        StateNamesView.SetRootItems(Items);
        StateNamesView.Rebuild();
    }
}
