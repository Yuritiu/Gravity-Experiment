using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LocalGravity : MonoBehaviour
{
    public Vector3 gravityDirection = Vector3.down;
    public float gravityStrength = 9.81f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // disable Unity's default gravity
    }

    void FixedUpdate()
    {
        rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);
    }

    public void SetGravity(Vector3 dir)
    {
        gravityDirection = dir.normalized;
    }
}
