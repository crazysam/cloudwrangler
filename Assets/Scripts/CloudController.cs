using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cloudColliderCenter;
    public float runThreshold;
    public float runVelocity;
    public float normalVelocity;
	
	[HideInInspector]
	public static int numRainingClouds = 0;

    public enum CloudState
    {
        Normal,
        Rain,
        Dead
    }

    public CloudState state;

    // Use this for initialization
    void Start()
    {
        particleSystem.enableEmission = false;
        state = CloudState.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == CloudState.Normal)
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
        else if (state == CloudState.Rain && cloudColliderCenter != null)
        {
            Vector3 deltaPosition = new Vector3(0, 0, 0);//transform.position - cloudColliderCenter.position;
            //deltaPosition.Normalize();
            //deltaPosition.y = 0.0f;
            rigidbody.velocity = deltaPosition * normalVelocity;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.Equals("CloudCollider"))
        {
            if (state == CloudState.Normal)
            {
                state = CloudState.Rain;
                particleSystem.enableEmission = true;
                cloudColliderCenter = collision.transform;
                numRainingClouds++;
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name.Equals("CloudCollider"))
        {
            if (state == CloudState.Rain)
            {
                state = CloudState.Dead;
                particleSystem.enableEmission = false;
                numRainingClouds--;
            }
        }
    }
}
