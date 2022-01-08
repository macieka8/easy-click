using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class InGameMenu : MonoBehaviour, IMenuController
    {
        PlayerControls _PlayerControls;

        LinkedList<GameObject> _WindowStack;
        [SerializeField] GameObject _StartMenu;

        private void Awake()
        {
            _WindowStack = new LinkedList<GameObject>();
            _PlayerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _PlayerControls.Gameplay.Back.performed += OnBack;
            _PlayerControls.Gameplay.Back.Enable();
        }

        private void OnDisable()
        {
            _PlayerControls.Gameplay.Back.Disable();
            _PlayerControls.Gameplay.Back.performed -= OnBack;
        }

        private void OnBack(InputAction.CallbackContext obj)
        {
            if (LevelLoader.CurrentLevel.StartsWith("Level "))
            {
                if (_WindowStack.Count > 0)
                {
                    Rollback();
                }
                else
                {
                    Time.timeScale = 0f;
                    ChangeWindow(_StartMenu);
                }
            }
        }

        public void ChangeWindow(GameObject window)
        {
            if (_WindowStack.Count > 0)
            {
                _WindowStack.Last.Value.SetActive(false);
            }
            _WindowStack.AddLast(window);
            window.SetActive(true);
        }

        public void Rollback()
        {
            if (_WindowStack.Count > 1)
            {
                _WindowStack.Last.Value.SetActive(false);
                _WindowStack.RemoveLast();

                _WindowStack.Last.Value.SetActive(true);
            }
            else if (_WindowStack.Count == 1)
            {
                _WindowStack.Last.Value.SetActive(false);
                _WindowStack.RemoveLast();

                Time.timeScale = 1f;
            }
        }

        public void SetTimeScale(float timeScale)
        {
            // TODO: create time manager
            Time.timeScale = timeScale;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}