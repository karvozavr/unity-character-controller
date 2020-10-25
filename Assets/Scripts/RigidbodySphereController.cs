using UnityEngine;

public class RigidbodySphereController : MonoBehaviour
{
    private Rigidbody _body;

    [SerializeField, Range(0f, 100f)] private float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 10f;

    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 10f)] private float jumpHeight = 2f;

    [SerializeField, Range(0, 5)] private int maxAirJumps = 0;

    [SerializeField, Range(0f, 90f)] private float maxGroundAngle = 25f;

    private Vector3 _velocity;

    private Vector3 _desiredVelocity;

    private bool _desiredJump;

    private int _jumpPhase;

    private bool _onTheGround;

    private float _minGroundDotProduct;

    private Vector3 _contactNormal;

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
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

    private void FixedUpdate()
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

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void EvaluateCollision(Collision collision)
    {
        foreach (var contactPoint in collision.contacts)
        {
            Vector3 normal = contactPoint.normal;
            if (normal.y >= _minGroundDotProduct)
            {
                _onTheGround = true;
                _contactNormal = normal;
            }
        }
    }

    private void Jump()
    {
        if (_onTheGround || _jumpPhase < maxAirJumps)
        {
            _jumpPhase += 1;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
            float alignedSpeed = Vector3.Dot(_velocity, _contactNormal);
            if (alignedSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
            }

            _velocity += _contactNormal * jumpSpeed;
        }
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (_onTheGround)
        {
            _jumpPhase = 0;
        }
        else
        {
            _contactNormal = Vector3.up;
        }
    }

    private void AdjustVelocity(Vector3 desiredVelocity)
    {
        float acceleration = _onTheGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);
        _body.velocity = _velocity;
    }
}