using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    #region Transform
    /// <summary>
    /// Returns the closet child to the object from any child
    /// </summary>
    public static Transform GetClosetChild(this Transform parent, Transform child)
    {
        Transform descendant = child;
        while (descendant.parent != null)
        {
            if (descendant.parent == parent)
            {
                // If we are the closet child, exit
                return descendant;
            }
            // Otherwise we go up the family tree
            descendant = descendant.parent;
        }
        return null;
    }

    /// <summary>
    /// Returns the index of closet child to the object from any child
    /// </summary>
    public static int GetClosetChildsIndex(this Transform parent, Transform child)
    {
        Transform output = parent.GetClosetChild(child);
        return output == null ? -1 : output.GetSiblingIndex();
    }
    #endregion

    #region Vector3
    /// <summary>
    /// Returns the Vector3 with the X modified
    /// </summary>
    public static Vector3 SetX(this Vector3 pos, float x)
    {
        pos.x = x;
        return pos;
    }

    /// <summary>
    /// Returns the Vector3 with the Y modified
    /// </summary>
    public static Vector3 SetY(this Vector3 pos, float y)
    {
        pos.y = y;
        return pos;
    }

    /// <summary>
    /// Returns the Vector3 with the Z modified
    /// </summary>
    public static Vector3 SetZ(this Vector3 pos, float z)
    {
        pos.z = z;
        return pos;
    }
    #endregion
}
