using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace EasyClick
{
    public class GameCharactersManager : MonoBehaviour
    {
        public static GameCharactersManager Instance;

        GameObject[] _PlayableCharacters;
        public static GameObject[] PlayableCharacters { get => Instance._PlayableCharacters; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        private void Start()
        {
            if (Instance == this)
            {
                _PlayableCharacters = Resources.LoadAll<GameObject>("Playable Characters");
            }
        }
    }
}