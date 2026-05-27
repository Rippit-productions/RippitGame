using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace StageSelect.UI
{
    public class StageSelectUI : MonoBehaviour, IMoveHandler, ISubmitHandler, ICancelHandler
    {
        [SerializeField] private SelectOption[] _Options;

        [Header("UI Setup")]
        [SerializeField] private Image StageImage;
        [SerializeField] private Image StageLogoImage;

        [Header("UI Controller")]
        [SerializeField] private GameObject UIControllerPrefab;

        public string SelectedStage => _Options[_SelectIndex].SceneName;
        private int _SelectIndex = 0;

        public UnityEvent OnCancelPressed = new UnityEvent();
        public UnityEvent OnStageSubmit = new UnityEvent();
        // Start is called before the first frame update
        void Start()
        {
            _Refresh();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void _Refresh()
        {
            StageImage.sprite = _Options[_SelectIndex].PreviewImage;
            StageLogoImage.sprite = StageImage.sprite = _Options[_SelectIndex].LogoImage;
        }

        public void OnCancel(BaseEventData eventData)
        {
            OnCancelPressed.Invoke();
        }

        public void OnMove(AxisEventData eventData)
        {
            var move = eventData.moveVector;
            _SelectIndex += (int)move.x;

            int maxValue = _Options.Length - 1;
            if (_SelectIndex > maxValue)
            {
                _SelectIndex = 0;
            }
            else if (_SelectIndex < 0) { }
            {
                _SelectIndex = maxValue;
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnStageSubmit.Invoke();
        }
    }
}