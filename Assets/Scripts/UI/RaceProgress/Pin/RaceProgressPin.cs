using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class RaceProgressPin : MonoBehaviour
{
    public static RaceProgressPin[] All => FindObjectsByType<RaceProgressPin>(FindObjectsInactive.Exclude,FindObjectsSortMode.InstanceID).ToArray();
    private int _index => transform.GetSiblingIndex();
    private Skater _targetSkater = null;

    RectTransform _rectTransform;
    TMP_Text Text;
    Image Icon;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        Skater.OnSkateDestroy += skater =>
        {
            if (skater == _targetSkater)
            {
                Destroy(this.gameObject);
            }
        };
    }

    public void SetTargetSkater(Skater target)
    {
        _targetSkater = target;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_targetSkater == null) return; 
        var playerInfo = RaceGameMode.Instance.GetProgressOfPlayer(_targetSkater);
        Vector2 pinPosition = new Vector2(playerInfo.Completion, 0.5f);

        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.anchorMin = pinPosition;
        _rectTransform.anchorMax = pinPosition;
    }

}
