using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(PolygonCollider2D))]
public class CameraBounds : MonoBehaviour
{
    public static CameraBounds Instance => FindFirstObjectByType<CameraBounds>();

    public PolygonCollider2D  Collider => GetComponent<PolygonCollider2D>();

}
