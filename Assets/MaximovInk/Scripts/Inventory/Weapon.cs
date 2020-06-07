using UnityEngine;

namespace MaximovInk
{
    public class Weapon : MonoBehaviour
    {
        public bool isActive;

        public void ActivateTrigger()
        {
            isActive = true;
        }

        public void DeactivateTrigger()
        {
            isActive = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive)
                return;
        }
    }
}