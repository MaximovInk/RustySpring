using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MaximovInk
{
    public class RotateToCamera : MonoBehaviour
    {
        public bool X, Y, Z;

        public float damping = 100f;

        private void LateUpdate()
        {
            var target = Camera.main.transform;

            Vector3 v = target.position - transform.position;
            v.y = v.z = 0.0f;
            transform.LookAt(target.position - v);
        }
    }
}