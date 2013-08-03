using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int moveSpeed;
	public int rotateSpeed;
	public Transform lasso;

	private LassoController lassoController;

	// Use this for initialization
	void Start()
	{
		lassoController = lasso.GetComponent<LassoController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (lassoController.isEngaged)
			return;

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
