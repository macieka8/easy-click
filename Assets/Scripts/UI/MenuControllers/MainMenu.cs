using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class MainMenu : MonoBehaviour, IMenuController
    {
        PlayerControls _PlayerControls;

        LinkedList<GameObject> _Windows = new LinkedList<GameObject>();
        [SerializeField] List<GameObject> _InitialNodes;

        private void Awake()
        {
            foreach (var node in _InitialNodes)
            {
                _Windows.AddLast(node);
            }
            _PlayerControls = new PlayerControls();
        }

        private void OnEnable()
        {
            _PlayerControls.Gameplay.Back.performed += Back_performed;
            _PlayerControls.Gameplay.Back.Enable();
        }

        private void OnDisable()
        {
            _PlayerControls.Gameplay.Back.Disable();
        }

        private void Back_performed(InputAction.CallbackContext obj)
        {
            Rollback();
        }

        public void ChangeWindow(GameObject window)
        {
            _Windows.Last.Value.SetActive(false);
            _Windows.AddLast(window);
            window.SetActive(true);
        }

        public void Rollback()
        {
            if (_Windows.Count > 1)
            {
                _Windows.Last.Value.SetActive(false);
                _Windows.RemoveLast();

                _Windows.Last.Value.SetActive(true);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}