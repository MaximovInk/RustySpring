using System;
using UnityEngine;

namespace MaximovInk
{
    public class ColliderCheckerPart : MonoBehaviour
    {
        public event Action CollisionEnter;

        public event Action CollisionExit;

        public event Action TriggerEnter;

        public event Action TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke();
        }

        private void OnCollisionEnter(Collision collision)
        {
            CollisionEnter?.Invoke();
        }

        private void OnCollisionExit(Collision collision)
        {
            CollisionExit?.Invoke();
        }
    }
}