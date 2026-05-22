using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using RippitGameManager;

namespace CharacterSelect.UI
{
    [Serializable]
    public struct CharacterOption
    {
        public Character character;
        public Sprite BannerImage;
    }
    public class CharacterSelectUI : MonoBehaviour, IMoveHandler, ICancelHandler, ISubmitHandler
    {
        private int _PlayerIndex = -1;
        [SerializeField] TMPro.TMP_Text _PlayerNumber;
        [SerializeField] Image _CharacterImage;

        public bool Confirmed = true;
        private bool _Confirmed = false;

        [SerializeField] private CanvasGroup _ConfirmedPrompt;

        public Character SelectedCharacter => Options[_SelectIndex].character;
        private int _SelectIndex = 0;
        [SerializeField] private CharacterOption[] Options;

        // Events
        public Action<Character> OnSelectionChange = character => { };
        public Action<CharacterSelectUI> OnBeforeDestroy = UI => { };

        // Start is called before the first frame update
        void Start()
        {
            
        }

        private void OnDestroy()
        {
            OnBeforeDestroy.Invoke(this);
        }

        public void SetPlayerIndex(int newIndex)
        {
            _PlayerIndex = newIndex;
        }

        private void _Refresh()
        {
            _CharacterImage.sprite = Options[_SelectIndex].BannerImage;

            if (_Confirmed)
            {
                _ConfirmedPrompt.alpha = 1;
            }
            else
            {
                _ConfirmedPrompt.alpha = 0;
            }
        }

        public void OnMove(AxisEventData eventData)
        {
            if (_PlayerIndex < 0 || _Confirmed) return;
            Vector2 move = eventData.moveVector;
            _SelectIndex += (int)move.x;

            int maxValue = Options.Length;

            if (_SelectIndex >= maxValue)
            {
                _SelectIndex = 0;
            }
            else if (_SelectIndex < 0)
            {
                _SelectIndex = maxValue - 1;
            }

            OnSelectionChange.Invoke(this.SelectedCharacter);

            if (GameManager.Instance.CharacterSelection.HasPlayer(_PlayerIndex))
            {
                var data = GameManager.Instance.CharacterSelection[_PlayerIndex];
                data.Character = this.SelectedCharacter;
                GameManager.Instance.CharacterSelection[_PlayerIndex] = data;
            }
            _Refresh();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _Confirmed = true;
            _Refresh();
        }

        public void OnCancel(BaseEventData eventData)
        {
            
            if (this._Confirmed == false)
            {
                GameObject.Destroy(this.gameObject);
            }
            else
            {
                this._Confirmed = false;
                _Refresh();
            }
        }

#if UNITY_EDITOR
        private int GuiID = Guid.NewGuid().GetHashCode();
        private Rect _GuiRect = new Rect(20, 20, 300, 50);
        void OnGUI()
        {
            _GuiRect = GUILayout.Window(GuiID, _GuiRect, _DrawGUIWindow, $"Character Select - {this._PlayerIndex}");
        }

        void _DrawGUIWindow(int WindowID)
        {
            if (GameManager.Instance.CharacterSelection.HasPlayer(this._PlayerIndex))
            {
                GUILayout.Label($"Selected Character - {GameManager.Instance.CharacterSelection[this._PlayerIndex].Character}");
            }
            GUI.DragWindow(new Rect(0, 0, float.MaxValue, float.MaxValue));
        }
#endif
    }
}