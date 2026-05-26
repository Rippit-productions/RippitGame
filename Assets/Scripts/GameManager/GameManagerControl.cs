using System.Collections;
using UnityEngine;

namespace RippitGameManager
{
    public class GameManagerControl : MonoBehaviour
    {
        public void LoadScene(string sceneName) => GameManager.Instance.LoadScene(sceneName);
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}