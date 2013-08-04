using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudController : MonoBehaviour
{
    public Transform playerTransform;
    [HideInInspector]
    public Transform cloudColliderCenter;
    public float runThreshold;
    public float runVelocity;
    public float normalXVelocity;
    public float normalZVelocity;
    public float pullVelocity;
	
	[HideInInspector]
	public static List<CloudController> rainingClouds = new List<CloudController>();

    public enum CloudState
    {
        Normal,
        Rain,
        Dead
    }

    [HideInInspector]
    public CloudState state;

    private int flip;

    // Use this for initialization
    void Start()
    {
        particleSystem.enableEmission = false;
        state = CloudState.Normal;
        flip = 1;
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
            else
            {
                
                rigidbody.velocity = new Vector3(flip * normalXVelocity, 0, flip * normalZVelocity);
            }
        }
        else if (state == CloudState.Rain && cloudColliderCenter != null)
        {
            Vector3 deltaPosition = cloudColliderCenter.position - transform.position;
            deltaPosition.y = 0.0f;

            float deltaMag = deltaPosition.magnitude;
            deltaPosition.Normalize();

            rigidbody.AddForce(deltaPosition * pullVelocity * deltaMag);
        }
        else if (state == CloudState.Dead)
        {
            rigidbody.velocity = Vector3.zero;
            particleSystem.enableEmission = false;
            gameObject.active = false;
        }
    }
	
	public void SetNormalState()
	{
        rigidbody.velocity = Vector3.zero;
        particleSystem.enableEmission = false;
		state = CloudState.Normal;
	}
	
	public void SetDeadState()
	{
        rigidbody.velocity = Vector3.zero;
        particleSystem.enableEmission = false;
		state = CloudState.Dead;
		renderer.enabled = false;
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Equals("Walls"))
        {
            print("WALL HIT");
            flip *= -1;
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
                rainingClouds.Add(this);
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
                rainingClouds.Remove(this);
            }
        }
    }
}
