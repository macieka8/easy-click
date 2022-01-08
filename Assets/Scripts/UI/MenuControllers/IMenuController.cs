using UnityEngine;

namespace EasyClick
{
    public interface IMenuController
    {
        public void ChangeWindow(GameObject window);
        public void Rollback();
    }
}