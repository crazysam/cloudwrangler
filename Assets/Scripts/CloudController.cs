using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
    public Transform playerTransform;
    public Transform cloudColliderCenter;
    public float runThreshold;
    public float runVelocity;
    public float normalVelocity;

    public Material idleMaterial;
    public Material rainMaterial;
	
	[HideInInspector]
	public static int numRainingClouds = 0;

    public enum CloudState
    {
        Normal,
        Rain,
        Dead
    }

    [HideInInspector]
    public CloudState state;

    // Use this for initialization
    void Start()
    {
        particleSystem.enableEmission = false;
        state = CloudState.Normal;
        renderer.material = idleMaterial;
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
            Vector3 deltaPosition = transform.position - cloudColliderCenter.position;
            deltaPosition.y = 0.0f;
            deltaPosition.Normalize();

            rigidbody.velocity = deltaPosition * normalVelocity;
            renderer.material = rainMaterial;
        }
        else if (state == CloudState.Dead)
        {
            rigidbody.velocity = Vector3.zero;
            particleSystem.enableEmission = false;
            renderer.enabled = false;
        }
    }
	
	void SetNormalState()
	{
		state = CloudState.Normal;
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
                print("Enter");
				print ("numRainingClouds="+numRainingClouds);
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name.Equals("CloudCollider"))
        {
            if (state == CloudState.Rain)
            {
                print("Exit");
                state = CloudState.Dead;
                particleSystem.enableEmission = false;
                print ("numRainingClouds="+numRainingClouds);
            }
        }
    }

    void SetCloudStateNormal()
    {
        state = CloudState.Normal;
    }
}
