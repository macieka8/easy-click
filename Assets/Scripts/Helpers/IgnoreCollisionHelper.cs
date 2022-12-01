using System.Collections.Generic;
using UnityEngine;

namespace EasyClick
{
    public static class IgnoreCollisionHelper
    {
        public static bool CheckIfNotIgnoredColliderExist(Collider2D[] foundColliders, List<Collider2D> ignoredColliders)
        {
            bool notIgnoredColliderFound = false;
            foreach (var foundCollider in foundColliders)
            {
                if (foundCollider != null && !ignoredColliders.Contains(foundCollider))
                {
                    notIgnoredColliderFound = true;
                    break;
                }
            }
            return notIgnoredColliderFound;
        }
    }
}
