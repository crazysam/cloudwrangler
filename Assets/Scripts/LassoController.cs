using UnityEngine;
using System.Collections;

public class LassoController : MonoBehaviour
{
	public int moveSpeed;
	public Transform lassoSpawnLocation;

	private bool isEngaged;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (isEngaged)
		{
			Debug.DrawLine(transform.position, Input.mousePosition);

			Vector3 worldMouseLocation = new Vector3(Input.mousePosition.x, transform.position.y, Input.mousePosition.z);
			Vector3 moveDirection = transform.position - worldMouseLocation;

			Debug.DrawLine(transform.position, worldMouseLocation);

			transform.position += moveDirection.normalized * (moveSpeed * Time.deltaTime);
		}

		ProcessInput();
	}

	void ProcessInput()
	{
		if (!isEngaged && Input.GetMouseButtonDown(0))
		{
			isEngaged = true;
			GetComponent<TrailRenderer>().enabled = true;
			transform.position = lassoSpawnLocation.position;
		}
		else if (isEngaged && Input.GetMouseButtonUp(0))
		{
			isEngaged = false;
			GetComponent<TrailRenderer>().enabled = false;
		}
	}
}
