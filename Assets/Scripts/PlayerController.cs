using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5.0f;

    [Header("Turn (90º smooth)")]
    [SerializeField] private float turnStepDegrees = 90f;
    [SerializeField] private float turnSpeedDegreesPerSec = 360f; // 360 = tarda 0.25s en girar 90º
    [SerializeField] private bool lockMovementWhileTurning = true; // evita "arcos" al avanzar mientras gira

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private bool enableDoubleJump = false;

    private Rigidbody rb;

    // Estado
    private bool isGrounded = false;
    private bool hasDoubleJumped = false;

    // Input
    private float forwardInput = 0f;

    // Rotación objetivo (en pasos)
    private Quaternion targetRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        targetRotation = rb.rotation;
    }

    private void Update()
    {
        // --- ROTACIÓN EN PASOS (pero aplicada suave en FixedUpdate) ---
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            AddTurn(+turnStepDegrees);

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            AddTurn(-turnStepDegrees);

        // --- MOVIMIENTO DELANTE/ATRÁS (sin diagonales) ---
        forwardInput = 0f;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            forwardInput = 1f;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            forwardInput = -1f;

        // --- SALTO ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                Jump();
                hasDoubleJumped = false;
            }
            else if (enableDoubleJump && !hasDoubleJumped)
            {
                Jump();
                hasDoubleJumped = true;
            }
        }
    }

    private void FixedUpdate()
    {
        // 1) Rotación suave hacia el objetivo
        bool isTurning = Quaternion.Angle(rb.rotation, targetRotation) > 0.1f;
        Quaternion newRot = Quaternion.RotateTowards(
            rb.rotation,
            targetRotation,
            turnSpeedDegreesPerSec * Time.fixedDeltaTime
        );
        rb.MoveRotation(newRot);

        // 2) Movimiento (opcionalmente bloqueado mientras gira)
        float moveInput = (lockMovementWhileTurning && isTurning) ? 0f : forwardInput;

        Vector3 delta = transform.forward * (moveInput * speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + delta);
    }

    private void AddTurn(float degrees)
    {
        Quaternion step = Quaternion.Euler(0f, degrees, 0f);
        targetRotation = targetRotation * step; // acumula giros aunque pulses varias veces
    }

    private void Jump()
    {
        // Si tu Unity no tiene linearVelocity, usa rb.velocity
        Vector3 v = rb.linearVelocity;
        v.y = 0f;
        rb.linearVelocity = v;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            hasDoubleJumped = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}