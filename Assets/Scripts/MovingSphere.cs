using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSphere : MonoBehaviour {

    [SerializeField, Range(0f, 100f)]
    public float maxSpeed = 10f;

    void Update() {
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1.0f);
        Vector3 velocity = new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
        Vector3 displacement = velocity * Time.deltaTime;
        transform.localPosition += displacement;
    }
}
