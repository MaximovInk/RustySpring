using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MaximovInk
{
    public class Player : Entity
    {
        public float SensitivityX { get; set; } = 2f;
        public float SensitivityY { get; set; } = 2f;
        public float Speed { get; set; } = 4f;
        public float BuildDistance { get; set; } = 50f;
        public float PlayerHeight { get; set; } = 1.1f;
        public float JumpForce { get; set; } = 8f;
        public LayerMask GroundMask { get; set; }
        public LayerMask InteractMask { get; set; }

        public Camera Camera { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        //public Animator Animator { get; private set; }
        public bool IsGround { get; private set; }

        private Vector2 move;
        private Vector2 look;

        private void Start()
        {
            Camera = GetComponentInChildren<Camera>();
            Rigidbody = GetComponent<Rigidbody>();

            GroundMask = ~((1 << (LayerMask.NameToLayer("Entity"))) | (1 << LayerMask.NameToLayer("Connections")));
            InteractMask = ~((1 << (LayerMask.NameToLayer("Entity"))) | (1 << LayerMask.NameToLayer("Connections")));
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private float rot;

        private void Update()
        {
            if (!Freeze)
            {
                move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * (Input.GetKey(KeyCode.LeftShift) ? 2 : 1);
                look = new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));

                transform.rotation *= Quaternion.Euler(0, look.x * SensitivityX, 0);
                rot += look.y;
                Camera.transform.localRotation = Quaternion.Euler(rot, 0, 0);
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Freeze = !Freeze;
                Cursor.lockState = Freeze ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = Freeze;
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (Physics.Raycast(new Ray(Camera.transform.position, Camera.transform.forward), out RaycastHit hit, Mathf.Infinity))
                {
                    var interactable = hit.collider.GetComponent<IInteractable>();

                    if (interactable != null)
                    {
                        interactable.Interact();
                    }
                }
            }
        }

        public bool Freeze { get => freeze; set { freeze = value; if (value) { Rigidbody.velocity = Vector3.zero; Rigidbody.angularVelocity = Vector3.zero; } } }
        private bool freeze;

        private void FixedUpdate()
        {
            IsGround = Physics.Raycast(transform.position, Vector3.down, PlayerHeight, GroundMask);

            if (Freeze)
                return;

            if (move.x != 0 || move.y != 0)
            {
                Rigidbody.velocity = (transform.forward * Speed * move.y) + (transform.right * Speed * move.x) + new Vector3(0, Rigidbody.velocity.y, 0);
            }
            if (look.x != 0)
                Rigidbody.rotation *= Quaternion.Euler(0, look.x, 0);

            if (IsGround && Input.GetKey(KeyCode.Space))
            {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpForce, Rigidbody.velocity.z);
            }
        }
    }
}