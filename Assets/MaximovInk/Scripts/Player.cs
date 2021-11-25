using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2 MouseSens = new Vector2(3, 3);
    public float Speed = 10;
    public float jumpForce = 250.0f;

    [SerializeField]
    private float CharacterLenght = 1;
    [SerializeField]
    private LayerMask groundedMask;

    private Rigidbody rb;
    private float verticalLookRotation;
    public Camera Camera{get=>_camera;}
    private Camera _camera;
    private Vector3 _force;
    private Vector3 moveVelocity;
    private bool grounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();

        /*
         
        Camera.main.transform.position = _camera.transform.position;
        Camera.main.transform.rotation = _camera.transform.rotation;
        Camera.main.transform.SetParent(_camera.transform.parent);
         */
    }


    private void Update()
    {
        var _moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        var _targetMove = _moveDir * Speed;
        _force = Vector3.SmoothDamp(_force, _targetMove, ref moveVelocity, 0.15f);

      

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
            rb.AddForce(transform.up * jumpForce);

        Ray ray = new Ray(transform.position, -transform.up);
        grounded = Physics.Raycast(ray, out _, CharacterLenght + .1f, groundedMask);

    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + transform.TransformDirection(_force) * Time.fixedDeltaTime);

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * MouseSens.x);
        verticalLookRotation += Input.GetAxis("Mouse Y") * MouseSens.y;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        _camera.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

}
