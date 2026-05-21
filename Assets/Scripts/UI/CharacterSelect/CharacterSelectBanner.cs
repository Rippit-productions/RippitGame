using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using CharacterSelect.Controller;
using System;
using System.Linq;

namespace CharacterSelect.UI
{
    [Serializable]
    public struct CharacterBanner
    {
        public Character character;
        public Sprite Banner;
    }
    public class CharacterSelectBanner : MonoBehaviour
    {

        private CharacterSelectController _controller;
        [SerializeField] TMPro.TMP_Text _PlayerNumber;
        [SerializeField] Image _CharacterImage;


        [SerializeField] private CharacterBanner[] Banners;
        // Start is called before the first frame update
        void Start()
        {
            
        }

        public void SetController(CharacterSelectController Controller)
        {
            _controller = Controller;
            _controller.OnSelectionChange += _Refresh;
        }

        private void _Refresh(Character newCharacter)
        {
            if (_controller == null) return;

            _PlayerNumber.text = $"P{_controller.PlayerIndex + 1}";

            var selectBanner = Banners.Where(b => b.character == newCharacter);
            if (selectBanner.Any()) {
                _CharacterImage.sprite = selectBanner.First().Banner;
            }
        }
    }
}