using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour
{
    [SerializeField] Rect allowedArea = new Rect(-5f, -5f, 10f, 10f);

    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)] float maxAcceleration = 10f;

    [SerializeField, Range(0f, 1f)] float bounciness = 0.5f;

    private Vector3 _velocity;

    private void Update()
    {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);

        Vector3 desiredVelocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;

        float maxSpeedChange = maxAcceleration * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, desiredVelocity.x, maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, desiredVelocity.z, maxSpeedChange);

        Vector3 displacement = _velocity * Time.deltaTime;
        Vector3 newPosition = transform.localPosition + displacement;
        newPosition = ProcessBoundsCollision(newPosition);

        transform.localPosition = newPosition;
    }

    private Vector3 ProcessBoundsCollision(Vector3 newPosition)
    {
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            _velocity.x = -_velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            _velocity.x = -_velocity.x * bounciness;
        }

        if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            _velocity.z = -_velocity.z * bounciness;
        }
        else if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            _velocity.z = -_velocity.z * bounciness;
        }

        return newPosition;
    }
}