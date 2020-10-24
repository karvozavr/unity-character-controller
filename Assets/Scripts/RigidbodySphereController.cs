using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySphereController : MonoBehaviour
{
    private Rigidbody body;

    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField, Range(0f, 10f)] 
    float jumpHeight = 2f;

    private Vector3 velocity;
    
    private Vector3 desiredVelocity;

    private bool desiredJump;

    private bool onTheGround;

    void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        desiredJump |= Input.GetButtonDown("Jump");
    }
    void FixedUpdate() 
    {
        velocity = body.velocity;
        if (desiredJump)
        {
            desiredJump = false;
            Jump();
        }
        AdjustVelocity(desiredVelocity);
        
        onTheGround = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }
    
    void OnCollisionStay (Collision collision) {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            Vector3 normal = contactPoint.normal;
            onTheGround |= normal.y >= 0.9f;
        }
    }
    
    private void Jump()
    {
        if (onTheGround)
        {
            velocity.y += Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
        }
    }
    
    private void AdjustVelocity(Vector3 desiredVelocity)
    {
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
        body.velocity = velocity;
    }
}
