using UnityEngine;

namespace MaximovInk
{
    public class BuilderController : MonoBehaviour
    {
        public Camera Camera { get; private set; }

        private Workbench workbench;

        private float movementSpeed = 5f;

        private bool active;

        public ITool CurrentTool;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
            workbench = GetComponentInParent<Workbench>();

            UpdateActive();
        }

        private void SelectBlock(string name)
        {
            CurrentTool?.OnDeselect();

            CurrentTool = new PlacingTool(new BlockPlacing(TileDatabase.GetBlock(name)));

            CurrentTool.OnSelect(this);
        }

        private void SelectObject(string name)
        {
            CurrentTool?.OnDeselect();

            CurrentTool = new PlacingTool(new ObjPlacing(TileDatabase.GetObject(name)));

            CurrentTool.OnSelect(this);
        }

        public void SetActive(bool active)
        {
            this.active = active;

            CurrentTool?.OnDeselect();
            if (active)
            {
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                CurrentTool = new PlacingTool(new BlockPlacing(TileDatabase.GetBlock("wood")));
                CurrentTool.OnSelect(this);
            }
            UpdateActive();
        }

        private void UpdateActive()
        {
            Camera.gameObject.SetActive(active);
        }

        private void Update()
        {
            if (!active)
                return;

            var depth = Input.GetKey(KeyCode.LeftShift) ? -1 : Input.GetKey(KeyCode.Space) ? 1 : 0;

            movementSpeed = Mathf.Max(movementSpeed += Input.GetAxis("Mouse ScrollWheel"), 0.0f);
            transform.position += (transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical") + transform.up * depth) * movementSpeed * Time.deltaTime;
            transform.eulerAngles += new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0);

            CurrentTool?.Update();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                var player = GameManager.instance.LocalPlayer;
                player.Freeze = false;
                player.Camera.enabled = true;
                SetActive(false);

                GameManager.instance.GameUI.SetActive(true);
                GameManager.instance.EditorUI.SetActive(false);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SelectBlock("wood");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SelectBlock("metal");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SelectObject("axle");
            }
        }
    }
}