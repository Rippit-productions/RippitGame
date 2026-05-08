using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using AnimationStateReference;



[CustomEditor(typeof(AnimatorStateReference))]
public class AnimatorStateReferenceEditor : EditorWindow
{
    [SerializeField]private VisualTreeAsset _EditorGUI;

    private GameObject _InitSelectedGameObj;

    private ObjectField ControllerField;
    private ScrollView StateScrollView;

    public AnimatorStateReference returnData => _returnData;
    private AnimatorStateReference _returnData;

    private class StateLabel : Label
    {
        private string _LayerName;
        private string _StateName;

        public string LayerName
        {
            set
            {
                _LayerName = value;
                text = GetFullString();
            }
        }

        public string StateName
        {
            set
            {
                _StateName = value;
                text = GetFullString();
            }
        }

        public string GetFullString()
        {
            return $"{_LayerName}.{_StateName}";
        }
    }

    private class LayerGroup : Foldout
    {
        private ListView LayerStateView;
        private List<AnimationStatePath> items = new List<AnimationStatePath>();

        public Action<AnimationStatePath> OnSelectionChange = newData => { };
        public LayerGroup(AnimatorControllerLayer Layer) 
        {
            this.value = false;
            this.text = Layer.name;

            foreach (var state in Layer.stateMachine.states)
            {
                items.Add(new AnimationStatePath()
                {
                    LayerName = Layer.name,
                    StateName = state.state.name
                });
            }

            Func<VisualElement> makeItem = () => new StateLabel();
            Action<VisualElement, int> bindItem = (Element, Index) =>
            {
                StateLabel label = (StateLabel)Element;
                label.LayerName = items[Index].LayerName;
                label.StateName = items[Index].StateName;
            };
            
            LayerStateView = new ListView(items,16,makeItem,bindItem);

            LayerStateView.selectedIndicesChanged += selection =>
            {
                var newData = items[LayerStateView.selectedIndices.First()];
                this.OnSelectionChange.Invoke(newData); 
            };

            this.Add(LayerStateView);

            this.RegisterValueChangedCallback(eventCallback =>
            {
                LayerStateView.Rebuild();
            });
        }
    }

    public Action<AnimatorStateReference> OnReturn = returnValue => { };
    public static  AnimatorStateReference PopUp(GameObject selectedGameObj = null)
    {
        var newWindow = ScriptableObject.CreateInstance<AnimatorStateReferenceEditor>();
        newWindow.titleContent.text = "Select State";
        newWindow._InitSelectedGameObj = selectedGameObj;
        newWindow.ShowModal();
        return newWindow.returnData;
    }

    private void CreateGUI()
    {
        var editorGUI = _EditorGUI.CloneTree();
        ControllerField = editorGUI.Q<ObjectField>("ControllerObjectField");
        StateScrollView = editorGUI.Q<ScrollView>("SateScrollView");

        ControllerField.RegisterValueChangedCallback(eventCallback =>
        {
            _Refresh();
        });

        if (this._InitSelectedGameObj) 
        {
            Animator animator = _InitSelectedGameObj.GetComponentInChildren<Animator>();
            AnimatorController controller = null;
            if (animator) 
            {
                controller = (AnimatorController)animator.runtimeAnimatorController;
                if (controller)
                {
                    ControllerField.value = controller;
                    _Refresh();
                }
            }
        }

        rootVisualElement.style.flexGrow = 1;
        editorGUI.style.flexGrow = 1;
        rootVisualElement.Add(editorGUI);
    }

    private void _Refresh()
    {
        if (ControllerField.value != null)
        {
            StateScrollView.Clear();
            var controller = (AnimatorController)ControllerField.value;
            foreach (var layer in controller.layers)
            {
                var newItem = new LayerGroup(layer);
                newItem.OnSelectionChange += (AnimationStatePath data) =>
                {
                    _returnData = new AnimatorStateReference(controller,data);
                    this.Close();
                };
                StateScrollView.Add(newItem);
            }
        }
    }

    private void OnDestroy()
    {
        
    }
}
