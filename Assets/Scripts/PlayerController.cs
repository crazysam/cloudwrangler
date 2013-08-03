using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int moveSpeed;
	public int rotateSpeed;
	public float lookAtDamping;

	private LassoController lassoController;

	// Use this for initialization
	void Start()
	{
		lassoController = GetComponentInChildren<LassoController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (lassoController.isEngaged)
			return;

		Vector3 forward = transform.TransformDirection(Vector3.forward);
		CharacterController controller = GetComponent <CharacterController>();

		int speed = moveSpeed;
		if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
			speed *= 2;

		// Move forward / backward
		if(Input.GetKey(KeyCode.W))
			controller.SimpleMove(forward * (speed * Time.deltaTime));
		else if(Input.GetKey(KeyCode.S))
			controller.SimpleMove(-forward * (speed * Time.deltaTime));
		
		
		
		if(lassoController.isHooked)
		{
			// Look at and dampen the rotation
			Quaternion rotation = Quaternion.LookRotation(lassoController.cloudCollider.transform.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamping);
			
			Vector3 left = transform.TransformDirection(Vector3.left);
			
			// Strafe
			if (Input.GetKey(KeyCode.A))
				controller.SimpleMove(left * (speed * Time.deltaTime));
			else if (Input.GetKey(KeyCode.D))
				controller.SimpleMove(-left * (speed * Time.deltaTime));
		}
		else
		{
			speed = rotateSpeed;
			if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
				speed *= 2;
			
			// Rotate around y - axis
			if (Input.GetKey(KeyCode.A))
				transform.Rotate(0, -speed * Time.deltaTime, 0);
			else if (Input.GetKey(KeyCode.D))
				transform.Rotate(0, speed * Time.deltaTime, 0);
		}
	}
}
