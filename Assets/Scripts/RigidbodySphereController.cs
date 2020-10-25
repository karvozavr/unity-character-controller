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

    private float _minGroundDotProduct;

    private Vector3 _contactNormal;

    private int _groundContactsCount;

    private bool OnGround => _groundContactsCount > 0;

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
        AdjustVelocity();
        
        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }
        
        _body.velocity = _velocity;
        ClearState();
    }

    private void ClearState () {
        _groundContactsCount = 0;
        _contactNormal = Vector3.zero;
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
                _groundContactsCount++;
                _contactNormal += normal;
            }
        }
    }

    private void Jump()
    {
        if (OnGround || _jumpPhase < maxAirJumps)
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
        if (OnGround)
        {
            _jumpPhase = 0;
            _contactNormal.Normalize();
        }
        else
        {
            _contactNormal = Vector3.up;
        }
    }

    private void GainDesiredVelocity(Vector3 desiredVelocity)
    {
        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);
        _body.velocity = _velocity;
    }

    private void AdjustVelocity()
    {
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, maxSpeedChange);

        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
    }

    private Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _contactNormal * Vector3.Dot(vector, _contactNormal);
    }
}