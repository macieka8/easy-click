using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public class SetParentOnTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IBody>(out var body))
            {
                collider.transform.SetParent(transform.parent);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.TryGetComponent<IBody>(out var body))
            {
                collider.transform.SetParent(null);
            }
        }
    }
}