using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 terrainSize;
    public Vector3 terrainPosition;
    public float runThreshold;
    public float runVelocity;
    public float normalVelocity;

    public enum CloudState
    {
        Normal,
        Squeeze,
        Dead
    }

    public CloudState state;

    // Use this for initialization
    void Start()
    {
        state = CloudState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaPosition = transform.position - playerTransform.position;

        if (deltaPosition.magnitude < runThreshold)
        {
            deltaPosition.Normalize();
            deltaPosition.y = 0.0f;
            rigidbody.velocity = deltaPosition * runVelocity;
        }
        else if (rigidbody.velocity.magnitude < 10.0f)
        {
            rigidbody.velocity = new Vector3(normalVelocity, 0, normalVelocity);
        }
    }
}
