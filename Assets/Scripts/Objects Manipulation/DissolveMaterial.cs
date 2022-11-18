using System.Collections;
using UnityEngine;

namespace EasyClick
{
    public class DissolveMaterial : MonoBehaviour
    {
        [SerializeField] CharacterRespawn _characterRespawn;
        Renderer _renderer;
        Material _material;
        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _material = _renderer.material;
            _material.SetFloat("_MaxHeight", _renderer.localBounds.center.y + _renderer.localBounds.extents.y);
            _material.SetFloat("_MinHeight", _renderer.localBounds.center.y - _renderer.localBounds.extents.y);
        }

        void OnEnable() => _characterRespawn.OnRespawnStarted += HandleRespawnStarted;
        void OnDisable() => _characterRespawn.OnRespawnStarted -= HandleRespawnStarted;

        void HandleRespawnStarted(float duration)
        {
            StartCoroutine(DissolveCoroutine(duration));
        }

        IEnumerator DissolveCoroutine(float duration)
        {
            float timeLeft = duration;
            while (timeLeft > 0f)
            {
                _material.SetFloat("_Progress", (duration - timeLeft) / duration);
                timeLeft -= Time.deltaTime;
                yield return null;
            }

            _material.SetFloat("_Progress", 0f);
        }
    }
}
