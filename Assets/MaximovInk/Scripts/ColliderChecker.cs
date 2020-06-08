using UnityEngine;

namespace MaximovInk
{
    public class ColliderChecker : MonoBehaviour
    {
        public int collisionDetected { get; private set; }

        public int triggerDetected
        {
            get; private set;
        }

        private MeshRenderer meshRenderer;

        private void Awake()
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();

            var colliders = GetComponentsInChildren<Collider>();
            for (int i = 0; i < colliders.Length; i++)
            {
                var checker = colliders[i].gameObject.AddComponent<ColliderCheckerPart>();
                checker.CollisionEnter += CollisionEnter;
                checker.CollisionExit += CollisionExit;
                checker.TriggerEnter += TriggerEnter;
                checker.TriggerExit += TriggerExit;
            }
        }

        private void UpdateMat()
        {
            if (collisionDetected > 0 || triggerDetected > 0)
            {
                meshRenderer.material.SetColor("_BaseColor", Color.red);
            }
            else
            {
                meshRenderer.material.SetColor("_BaseColor", new Color(1f, 1f, 1f, 0.5f));
            }
        }

        public void ResetCounters()
        {
            collisionDetected = 0;
            triggerDetected = 0;
        }

        private void CollisionEnter()
        {
            collisionDetected++;
            UpdateMat();
        }

        private void CollisionExit()
        {
            collisionDetected--;
            UpdateMat();
        }

        private void TriggerEnter()
        {
            triggerDetected++;
            UpdateMat();
        }

        private void TriggerExit()
        {
            triggerDetected--;
            UpdateMat();
        }
    }
}