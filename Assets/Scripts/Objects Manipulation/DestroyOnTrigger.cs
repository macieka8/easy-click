using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class DestroyOnTrigger : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> _ObjectsToDestroy;

        private void OnTriggerEnter2D(Collider2D collider)
        {
            int index = _ObjectsToDestroy.IndexOf(collider.gameObject);
            if (index > -1)
            {
                _ObjectsToDestroy.RemoveAt(index);
                Destroy(collider.gameObject);
            }
        }
    }
}