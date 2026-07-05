using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Gravity Settings")]
    public float gravityStrength = 9.81f;   // Gravity acceleration (m/s²)
    public Vector3 gravityDir = Vector3.down;

    [Header("Smooth Rotation Settings")]
    public float rotateDuration = 0.5f;     // How long the flip takes

    private Rigidbody rb;
    private bool isRotating = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We'll handle gravity manually
        Physics.gravity = gravityDir * gravityStrength;
    }

    void Update()
    {
        if (GravitySelector.selectedObject != null || isRotating)
            return;

        // Change gravity direction with keys
        if (Input.GetKeyDown(KeyCode.Q))
        {
            gravityDir = Vector3.down;
            StartCoroutine(SmoothAlignToGravity());
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            gravityDir = Vector3.up;
            StartCoroutine(SmoothAlignToGravity());
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            gravityDir = -transform.right;
            StartCoroutine(SmoothAlignToGravity());
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            gravityDir = transform.right;
            StartCoroutine(SmoothAlignToGravity());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            gravityDir = transform.forward;
            StartCoroutine(SmoothAlignToGravity());
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gravityDir = -transform.forward;
            StartCoroutine(SmoothAlignToGravity());
        }
    }

    void FixedUpdate()
    {
        // Apply gravity acceleration manually
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    IEnumerator SmoothAlignToGravity()
    {
        isRotating = true;

        Quaternion startRot = transform.rotation;
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, -gravityDir) * transform.rotation;

        // Update Unity's global gravity for consistency
        Physics.gravity = gravityDir * gravityStrength;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / rotateDuration;
            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        transform.rotation = targetRot;
        isRotating = false;
    }
}
