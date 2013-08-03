using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int moveSpeed;
	public int rotateSpeed;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		CharacterController controller = GetComponent < CharacterController>();

		int speed = moveSpeed;
		if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
			speed *= 2;

		// Move forward / backward
		if(Input.GetKey(KeyCode.W))
			controller.SimpleMove(forward * (speed * Time.deltaTime));
		else if(Input.GetKey(KeyCode.S))
			controller.SimpleMove(-forward * (speed * Time.deltaTime));

		// Rotate around y - axis
		if (Input.GetKey(KeyCode.A))
			transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
		else if (Input.GetKey(KeyCode.D))
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
	}
}
