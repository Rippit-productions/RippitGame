using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class CanvasSwitcher : MonoBehaviour
{
    // If you really wanted to use fields this is the way
    [field:SerializeField]
    public int _ActiveIndex;
    public int DefaultIndex = 0;
    private void Awake()
    {
        _Refresh();
    }
    
    private void Update()
    {
        _Refresh();
    }

    public void _Refresh()
    {

#if UNITY_EDITOR
        // Can't referance editor in game builds
        if (!Application.isPlaying) 
        {
            GameObject selected = UnityEditor.Selection.activeGameObject;

            if (selected != null && selected.transform != transform)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (selected.transform.IsChildOf(transform.GetChild(i)) ||
                        selected.transform == transform.GetChild(i))
                    {
                        _ActiveIndex = i;
                    }
                }
            }
            else
            {
                _ActiveIndex = this.DefaultIndex;
            }
        }
#endif

        // Toggle 
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _ActiveIndex);
        }
    }

    
    public void SetActiveIndex(int newIndex)
    {
        //if (newIndex < 0 || newIndex >= transform.childCount) return;
        _ActiveIndex = newIndex;
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
