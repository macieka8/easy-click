using UnityEngine;

namespace EasyClick
{
    public class RespawnOnEnter : MonoBehaviour
    {
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<CharacterRespawn>(out var foundCharacter))
            {
                foundCharacter.ForceRespawn();
            }
        }
    }
}
