using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(RectTransform))]
public class RaceMapPin : MonoBehaviour
{
    RaceMapPin[] All => FindObjectsByType<RaceMapPin>(FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID).ToArray();
    public static Action OnPinSpawn = () => _RefreshAll();

    RectTransform _rectTransform;
    TMP_Text Text;
    Image Icon;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        OnPinSpawn.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var if asd
        _rectTransform.anchorMin
    }

    private void Refresh()
    {

    }

    private static void _RefreshAll()
    {

    }
}
