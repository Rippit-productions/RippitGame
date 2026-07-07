using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


namespace CharacterSelect.UI
{
    public class CharacterJoinPrompt : MonoBehaviour
    {

        private float InitWidth; 
        [SerializeField] private CanvasGroup _CanvasGroup;
        [SerializeField] private LayoutElement _LayoutElement;
        // Start is called before the first frame update

        private void Start()
        {
        }

        void Update()
        {
            transform.SetSiblingIndex(int.MaxValue);
            if (CharacterSelectScene.Instance.DeviceQueue.Length == 0)
            {
                _CanvasGroup.alpha = 0.0f;

            }else
            {
                _CanvasGroup.alpha = 1.0f;
            }
        }
    }
}