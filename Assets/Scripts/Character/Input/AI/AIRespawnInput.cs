using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace EasyClick
{
    public class AIRespawnInput : MonoBehaviour, IRespawnInput
    {
        public event Action<IInputData> onRespawn;

        private void Start()
        {
            Respawn();
        }

        async void Respawn()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            onRespawn.Invoke(new MovementInputData(0f));

            Respawn();
        }
    }
}
