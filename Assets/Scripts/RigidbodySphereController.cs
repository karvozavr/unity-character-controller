using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodySphereController : MonoBehaviour
{
    private Rigidbody _body;

    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;

    [SerializeField, Range(0f, 100f)] float maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)] float jumpHeight = 2f;

    [SerializeField, Range(0, 5)] int maxAirJumps = 0;

    private Vector3 _velocity;

    private Vector3 _desiredVelocity;

    private bool _desiredJump;

    private int _jumpPhase;

    private bool _onTheGround;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        _desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        _desiredJump |= Input.GetButtonDown("Jump");
    }

    void FixedUpdate()
    {
        UpdateState();
        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }

        AdjustVelocity(_desiredVelocity);

        _onTheGround = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            Vector3 normal = contactPoint.normal;
            _onTheGround |= normal.y >= 0.9f;
        }
    }

    private void Jump()
    {
        if (_onTheGround || _jumpPhase < maxAirJumps)
        {
            _jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            if (_velocity.y > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - _velocity.y, 0f);
            }

            _velocity.y += jumpSpeed;
        }
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (_onTheGround)
        {
            _jumpPhase = 0;
        }
    }

    private void AdjustVelocity(Vector3 desiredVelocity)
    {
        float acceleration = _onTheGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);
        _body.velocity = _velocity;
    }
}