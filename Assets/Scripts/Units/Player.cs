using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    [SerializeField] Camera cam;
    [SerializeField] Transform orientation;
    private PlayerInput playerInput;

    [SerializeField] Collider playerCollider;
    protected Rigidbody rb;

    [Header("Movement")]
    [SerializeField] float rotationSpeed = 10;
    bool readyToJump = true;
    float distToGround;

    [Header("Bullets")]
    [SerializeField] private int maxBullets = 10;
    [SerializeField] private int bulletCount = 10;
    [SerializeField] float bulletSpeed = 10f;
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.PlayerMain.Enable();
    }
    private void Start()
    {
        distToGround = playerCollider.bounds.extents.y;
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        UpdateMovement();
        AddDrag();
        SpeedControl();
        SenseEnemy();
    }

    // Get view direction from camera transform
    void GetViewDirection()
    {
        Vector3 camPosition = cam.transform.position;
        Vector3 playerPosition = transform.position;
        Vector3 viewDir = transform.position - new Vector3(camPosition.x, playerPosition.y, camPosition.z);
        orientation.forward = viewDir.normalized;
    }

    // Get Input direction using camera direction and input actions
    Vector3 GetInputDirection()
    {
        GetViewDirection();
        Vector2 inputVec = GetInputVector();
        return orientation.forward * inputVec.y + orientation.right * inputVec.x;
    }

    // Get input vector from input actions
    Vector2 GetInputVector()
    {
        Vector2 inputVector = playerInput.PlayerMain.Move.ReadValue<Vector2>();

        return inputVector;
    }

    // Add drag to physics
    void AddDrag()
    {
        if (isGrounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    // Check if player is on the ground
    bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
    }

    // Reset jump state
    void ResetJump()
    {
        readyToJump = true;
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    void SenseEnemy()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<Agent>(out Agent agent))
            {

                //Debug.Log("overlap " + collider);
                transform.LookAt(agent.transform);
                //gunScript.shoot();
            }
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (bulletCount > 0)
        {
            --bulletCount;
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            bullet.GetComponent<Rigidbody>().velocity = bulletSpawnPoint.forward * bulletSpeed;
        }
    }

    public void TakeHit(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            ResetGame();
        }
    }

    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Handle jump action
    private void Jump()
    {
        if (readyToJump && isGrounded())
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jump " + rb);
            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    // Move player in the moveDirection
    void MovePlayer(Vector3 moveDirection)
    {
        if (isGrounded())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    // Update heading direction
    void UpdateMovement()
    {
        Vector3 inputDirection = GetInputDirection().normalized;
        if (inputDirection != Vector3.zero)
        {
            MovePlayer(inputDirection);
            transform.forward = Vector3.Slerp(transform.forward, inputDirection, Time.deltaTime * rotationSpeed);
        }
    }
}
