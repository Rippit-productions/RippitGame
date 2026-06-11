using AnimationStateReference;
using System.Collections;
using UnityEngine;


namespace GameLevel.Intro
{
    public enum LevelCameraMode
    {
        Intro,
        Ending
    }
    [RequireComponent(typeof(Animator))]
    public class LevelCamera : MonoBehaviour
    {
        public bool Active => _Active;
        private bool _Active;

        public LevelCameraMode CameraMode => _Mode;
        private LevelCameraMode _Mode;


        AnimatorStateReference IntroAnimation;
        AnimatorStateReference ExitAnimation;

        [SerializeField] private Camera _Camera;
        [SerializeField] private Animator _Animator;

        // Use this for initialization
        void Start()
        {
            _Camera = GetComponentInChildren<Camera>();
            _Animator = GetComponentInChildren<Animator>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetMode(LevelCameraMode mode)
        {
            _Mode = mode;

            switch (_Mode)
            {
                case LevelCameraMode.Intro:
                    _Animator.Play(IntroAnimation);
                    break;
                case LevelCameraMode.Ending:
                    _Animator.Play(ExitAnimation);
                    break;
            }
        }

        public void SetActive(bool Active)
        {
            _Active = Active;
            switch (_Active)
            {
                case true:
                    _Camera.depth = float.MinValue;
                    break;
                case false:
                    _Camera.depth = float.MaxValue;
                    break;
            }
        }
    }
}