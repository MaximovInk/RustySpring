using UnityEngine;
namespace MaximovInk
{
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.transform.position += new Vector3(0, 150, 0);

            var rb = other.gameObject.GetComponent<Rigidbody>();

            if (rb != null)
                rb.velocity = Vector3.zero;
        }
    }
}