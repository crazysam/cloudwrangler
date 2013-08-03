using UnityEngine;
using System.Collections;

public class LassoController : MonoBehaviour
{
	public int moveSpeed;
	public float trailLifetime;
	public Transform lassoSpawnLocation;

	[HideInInspector]
	public bool isEngaged;
	private float lifetimeCounter;

	// Use this for initialization
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (isEngaged)
		{
			lifetimeCounter -= Time.deltaTime;
			if (lifetimeCounter > 0)
			{
				RaycastHit hitInfo = new RaycastHit();
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(mouseRay, out hitInfo))
				{
					Vector3 worldMouseLocation = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
					transform.position = Vector3.MoveTowards(transform.position, worldMouseLocation, Time.deltaTime * moveSpeed);
				}
				else
					DisengageLasso();
			}
			else
				DisengageLasso();
		}

		// Process Input
		if (!isEngaged && Input.GetMouseButtonDown(0))
			EngageLasso();
		else if (isEngaged && Input.GetMouseButtonUp(0))
			DisengageLasso();
	}

	private void EngageLasso()
	{
		isEngaged = true;
		lifetimeCounter = trailLifetime;
		GetComponent<TrailRenderer>().time = trailLifetime;
		transform.position = lassoSpawnLocation.position;
	}

	private void DisengageLasso()
	{
		isEngaged = false;
		GetComponent<TrailRenderer>().time = 0;
	}
}
