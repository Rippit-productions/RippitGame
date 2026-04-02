using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class CanvasSwitcher : MonoBehaviour
{
    // If you really wanted to use fields this is the way
    [field:SerializeField]
    public int ActiveIndex;

    private void Awake()
    {
        _Refresh();
    }
    
    private void Update()
    {
        _Refresh();
    }

    private void _Refresh()
    {

#if UNITY_EDITOR
        // Can't referance editor in game builds
        if (!Application.isPlaying) 
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;
            
            
            if (selected != null
                && selected.transform != transform
                && selected.transform.IsChildOf(transform))
            {
                ActiveIndex = selected.transform.GetSiblingIndex();
            }
            else
            {
                ActiveIndex = 0;
            }
        }
#endif

        // Toggle 
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == ActiveIndex);
        }
    }

    
    public void SetActiveIndex(int newIndex)
    {
        ActiveIndex = newIndex;
        Debug.Log("ActiveIndex: " + ActiveIndex);
        _Refresh();
    }


    /// <summary>
    /// Set Canvas layer to show target Child Object.
    /// Object must be child of this Canvas Switcher
    /// </summary>
    /// <param name="targetObj">The Child Object to Show</param>
    public void SwitchToObject(GameObject targetObj)
    {
        if (targetObj.transform.IsChildOf(this.gameObject.transform))
        {
            SetActiveIndex(this.transform.GetSiblingIndex());
        }
        // Target Object is not child of Switcher. Do nothing.
        else
        {
            return;
        }
    }
}
