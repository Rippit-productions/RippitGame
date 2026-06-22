using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace CharacterSelect.UI
{
    public class CharacterJoinPrompt : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            CharacterSelectUI.OnSpawn += _Refresh;
            CharacterSelectUI.OnUserCancel += ui => { 
                _Refresh();
            };
        }

        private void _Refresh()
        {
            transform.SetSiblingIndex(int.MaxValue); 
            if (CharacterSelectMenu.Instance.DeviceQueue.Length == 0)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                this.gameObject.SetActive(true);
            }
        }
    }
}