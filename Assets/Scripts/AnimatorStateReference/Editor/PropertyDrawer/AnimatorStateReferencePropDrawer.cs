using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using AnimationStateReference;

[CustomPropertyDrawer(typeof(AnimatorStateReference))]
public class AnimatorStateReferencePropDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        var root = new VisualElement();
        //Test UI elements
        var propDrawer = ScriptableObject.CreateInstance<AnimatorStateReferencePropDrawerGUI>();
        propDrawer.SetProperty(property);
        root.Add(propDrawer.GUI);
        return root;
    }
}
