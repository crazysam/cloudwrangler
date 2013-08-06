using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{
	public Transform playerTransform;
	public Transform cloudMeshTransform;
	public Transform floraPrefab;
	public LayerMask floraRaycastMask = -1;
    public Material happyCloudMat;
	public Material rainyCloudMat;
	public float moveVelocity = 100;
	public float moveAwayForce = 1;
	public float moveAwayDistanceThreshold = 20;
	public float lookAtPlayerForce = 1;
	public float idleMovementIntensity = 1;
	public float pullVelocity = 10000;
	public float minCloudScale = 0.1f;
	public float minStartScale = 0.75f;
	public float maxStartScale = 1f;
	public float minShrinkSpeed = 0.2f;
	public float maxShrinkSpeed = 0.5f;
	
	[HideInInspector]
	private float shrinkSpeed;
	[HideInInspector]
	private float cloudScale;
	[HideInInspector]
	private Transform cloudColliderCenter;

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
        state = CloudState.Normal;
		particleSystem.enableEmission = false;
		ResetVelocity();
		
		shrinkSpeed = Random.Range(minShrinkSpeed, maxShrinkSpeed);
		cloudScale = Random.Range(minStartScale, maxStartScale);
		transform.localScale = new Vector3(cloudScale, cloudScale, cloudScale);
		Quaternion newRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);
		transform.rotation = newRot;
    }
	
	void ResetVelocity()
	{
		rigidbody.velocity = new Vector3(Random.Range(-moveVelocity, moveVelocity), 0, Random.Range(-moveVelocity, moveVelocity));
	}

    // Update is called once per frame
    void Update()
    {
		if(GameController.state != GameController.GameState.play) return;
		
		if (state == CloudState.Normal && playerTransform != null)
		{
			// Look at player and try to move away from player if he's close enough
			if(Vector3.Distance(transform.position, playerTransform.position) <= moveAwayDistanceThreshold)
			{
				Quaternion rotation = Quaternion.LookRotation(playerTransform.position - transform.position);
				rotation.x = 0;
				rotation.z = 0;
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtPlayerForce);
				
				Vector3 playerDir = transform.position - playerTransform.position;
				Vector3 randomDir = new Vector3(Random.Range(-idleMovementIntensity, idleMovementIntensity), 0, Random.Range(-idleMovementIntensity, idleMovementIntensity));
				rigidbody.AddForce((playerDir.normalized * moveAwayForce) + randomDir);
			}
		}
        else if (state == CloudState.Rain)
        {
			// Add force to keep cloud within cloudCollider
			float colliderRadius = cloudColliderCenter.localScale.x;
            Vector3 deltaPosition = cloudColliderCenter.position - transform.position;
            deltaPosition.y = 0.0f;
			
            float magnitudeFromCenter = deltaPosition.magnitude / (colliderRadius / 2);
			if(!float.IsInfinity(magnitudeFromCenter))
			{
            	rigidbody.AddForce(deltaPosition.normalized * pullVelocity * magnitudeFromCenter);
			}
			
			// Spawn flora object directly underneath cloud
			RaycastHit hitInfo = new RaycastHit();
			Ray mouseRay = new Ray(transform.position, Vector3.down);
			if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, floraRaycastMask))
			{
				if(hitInfo.collider.tag != "FloraTag")
				{
					Quaternion newRot = Quaternion.Euler(90, 0, Random.Range(0, 360f));
					Instantiate(floraPrefab, new Vector3(hitInfo.point.x, hitInfo.point.y + 0.1f, hitInfo.point.z), newRot);
					GameController.score++;
				}
			}
			
			// Shrink cloud
			if(cloudScale > minCloudScale)
			{
				cloudScale -= Time.deltaTime * shrinkSpeed;
				transform.localScale = new Vector3(cloudScale, cloudScale, cloudScale);
			}
			else
			{
				CloudManager.rainingClouds.Remove(this);
				Destroy(gameObject);
			}
        }
    }

	public void SetNormalState()
	{
        rigidbody.velocity = Vector3.zero;
        particleSystem.enableEmission = false;
		state = CloudState.Normal;
		cloudMeshTransform.GetComponent<SkinnedMeshRenderer>().material = happyCloudMat;
	}
	
	public void SetDeadState()
	{
        state = CloudState.Dead;
        particleSystem.enableEmission = false;
        rigidbody.velocity = Vector3.zero;
        gameObject.SetActive(false);
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("WallTag"))
        {
			ResetVelocity();
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.name.Equals("CloudCollider"))
        {
            if (state == CloudState.Normal)
            {
                state = CloudState.Rain;
				cloudMeshTransform.GetComponent<SkinnedMeshRenderer>().material = rainyCloudMat;
                particleSystem.enableEmission = true;
                cloudColliderCenter = collision.transform;
                CloudManager.rainingClouds.Add(this);
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.name.Equals("CloudCollider"))
        {
            if (state == CloudState.Rain)
            {
				SetNormalState();
                CloudManager.rainingClouds.Remove(this);
            }
        }
    }
}
