using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Why is this here?
// * Reduce number of ticking background elements
// * Make it flexable of the future
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField]
    private GameObject _player;

    [Header("Parallax Controls")]
    [SerializeField]
    private int _scale = 3;

    [SerializeField]
    private List<ParallaxData> _parallaxPanels = new();

    private void Start()
    {
        foreach (ParallaxData pd in _parallaxPanels)
        {
            pd.Init(_scale);
        }
    }

    private void LateUpdate()
    {
        if (_player == null) { return; }
        
        foreach (ParallaxData pd in _parallaxPanels)
        {
            pd.Update(_player.transform.position.x);
        }
    }
    
    // Is an editor tool
    [ContextMenu("Auto Add Children To List")]
    private void AddChildrenToList()
    {
        int count = 0;
        Debug.Log("Searching for new children...");
        foreach (Transform child in transform)
        {
            bool isNewChild = true;

            foreach (ParallaxData pd in _parallaxPanels)
            {
                if (child.gameObject == pd.panel)
                {
                    isNewChild = false;
                    break;
                }
            }

            if (isNewChild)
            {
                //Debug.Log($"Found: {child.name}");

                ParallaxData newPD = new()
                {
                    panel = child.gameObject
                };

                _parallaxPanels.Add(newPD);
                count++;
            }
        }

        Debug.Log($"Added {(count == 0 ? "no" : count)} new objects");
    }
}

[System.Serializable]
public class ParallaxData
{
    public GameObject panel;
    public float percentageParallax = 0f;

    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    private float _startX = 0;
    private float _width = 0;

    public void Init(float scale)
    {
        _transform = panel.transform;
        _spriteRenderer = panel.GetComponent<SpriteRenderer>();
        _startX = _transform.localPosition.x;
        _width = _spriteRenderer.bounds.size.x;
        _spriteRenderer.size = new Vector2(_spriteRenderer.size.x * scale, _spriteRenderer.size.y);
    }

    public void Update(float x)
    {
        float temp = x * (1 - percentageParallax);
        float dist = x * percentageParallax;

        _transform.position = _transform.position.SetX(_startX + dist);
        
        if (temp > _startX + _width / 2)
        {
            _startX += _width;
        }
        else if (temp < _startX - _width / 2)
        {
            _startX -= _width;
        }
    }
}
