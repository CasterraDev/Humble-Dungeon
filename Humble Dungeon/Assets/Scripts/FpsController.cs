using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FpsController : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCam;
    Vector2 mouseLook;
    Vector2 smoothV;

    public float setSensitivity = 5f;
    public float smoothing = 2f;

    private float sensitivity;

    [Header("Movement")]
    public float speed = 5f;
    public float runMultipler = 2f;
    public float jumpForce = 200f;
    public KeyCode runKey = KeyCode.LeftShift;
    public LayerMask whatIsGround;
    public GameObject inventoryGO;

    [HideInInspector]
    public bool stop; //stops movement and camera control and also unlocks mouse
    private Vector3 standingLocalScale;
    public Vector3 crouchingLocalScale;
    public float slidingForce;

    float currentSpeed;

    [HideInInspector]
    public bool m_jump, grounded;

    private Rigidbody rb;
    public CapsuleCollider m_capsule;
    private Vector3 m_GroundContactNormal;
    public GameController GC;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        m_capsule = GetComponent<CapsuleCollider>();
        standingLocalScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            GC.menuStopped = !GC.menuStopped;
        }

        if (!GC.menuStopped)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Look();
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void FixedUpdate()
    {
        grounded = Physics.OverlapSphere(new Vector3(transform.position.x, transform.position.y - m_capsule.bounds.extents.y, transform.position.z), .1f, whatIsGround).Length > 0;

        if (!GC.menuStopped)
        {
            Movement();

            if (Input.GetButtonDown("Jump") && grounded)
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Look()
    {
        var md = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (!grounded)
        {
            sensitivity = Mathf.Max(1f, setSensitivity / 2);
        }
        else
        {
            sensitivity = setSensitivity;
        }

        md = Vector2.Scale(md, new Vector2(smoothing * sensitivity, smoothing * sensitivity));
        smoothV.x = Mathf.Lerp(smoothV.x, md.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, md.y, 1f / smoothing);
        mouseLook += smoothV;


        playerCam.transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
    }

    private void Movement()
    {
        if (Input.GetKey(runKey))
        {
            currentSpeed = speed * runMultipler;
        }
        else
        {
            currentSpeed = speed;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            Crouch();
        }
        else if (!Input.GetButton("Crouch"))
        {
            Stand();
        }

        float forwardBackInput = Input.GetAxis("Horizontal") * currentSpeed * Time.fixedDeltaTime;
        float strafe = Input.GetAxis("Vertical") * currentSpeed * Time.fixedDeltaTime;

        transform.Translate(forwardBackInput, 0, strafe);
    }

    void Crouch()
    {
        transform.localScale = crouchingLocalScale;
        rb.AddForce(transform.forward * slidingForce);
    }

    void Stand()
    {
        transform.localScale = standingLocalScale;
    }
}
