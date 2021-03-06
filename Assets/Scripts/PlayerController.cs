﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public int moveSpeed = 12500;
	public int hookedMoveSpeed = 14000;
	public int rotateSpeed = 75;
	public float lookAtDamping = 10;

	private LassoController lassoController;
	private Transform cloudCollider 
	{ 
		get { return lassoController.cloudCollider.transform; }
	}

	// Use this for initialization
	void Start()
	{
		lassoController = GetComponentInChildren<LassoController>();
	}

	// Update is called once per frame
	void Update()
	{
		if (lassoController.isEngaged || GameController.state != GameController.GameState.play)
			return;
		
		CharacterController controller = GetComponent <CharacterController>();
		Vector3 forward = transform.TransformDirection(Vector3.forward);
		Vector3 left = transform.TransformDirection(Vector3.left);
		
		// get distance to collider so we can calculate how much we should displace it by if player is pulling it
		float prevColliderDistance = lassoController.isHooked ? Vector3.Distance(transform.position, cloudCollider.position) : 0;
		
		if(lassoController.isHooked)
		{
			Vector3 moveDir = new Vector3();
			
			float colliderDistance = Vector3.Distance(transform.position, cloudCollider.position);
			
			// Move forward / backward - do not allow player to move inside cloudCollider
			if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && colliderDistance > Mathf.Max(cloudCollider.localScale.x, 125))
				moveDir += forward;
			else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				moveDir -= forward;
			
			// Strafe
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				moveDir += left;
			else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				moveDir -= left;
			
			int speed = hookedMoveSpeed;
			if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
				speed *= 2;
			
			controller.SimpleMove(moveDir * (hookedMoveSpeed * Time.deltaTime));
			
			// pull collider along
			colliderDistance = Vector3.Distance(transform.position, cloudCollider.position);
			float distanceDelta = colliderDistance - prevColliderDistance;
			if(distanceDelta > 0)
			{
				Vector3 colliderToPlayerDir = cloudCollider.position - transform.position;
				cloudCollider.position -= colliderToPlayerDir.normalized * distanceDelta;
			}
			
			CalcLassoLineLocation();
		}
		else
		{
			int speed = moveSpeed;
			if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
				speed *= 2;
			
			// Move forward / backward
			if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
				controller.SimpleMove(forward * (speed * Time.deltaTime));
			else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
				controller.SimpleMove(-forward * (speed * Time.deltaTime));
			
			speed = rotateSpeed;
			if (Debug.isDebugBuild && Input.GetKey(KeyCode.LeftShift))
				speed *= 2;
			
			// Rotate around y - axis
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
				transform.Rotate(0, -speed * Time.deltaTime, 0);
			else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
				transform.Rotate(0, speed * Time.deltaTime, 0);
		}
		
		// Look at collider if we caught some clouds!
		if(cloudCollider.gameObject.activeSelf && CloudManager.rainingClouds.Count > 0)
		{
			Quaternion rotation = Quaternion.LookRotation(cloudCollider.position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookAtDamping);
		}
	}
	
	public void CalcLassoLineLocation()
	{
		Vector3 colliderToPlayer = transform.position - cloudCollider.transform.position;
		Vector3 lineEndPos = cloudCollider.transform.position + (colliderToPlayer.normalized * (cloudCollider.localScale.x / 2));
		lassoController.lassoLine.GetComponent<LineRenderer>().SetPosition(0, lassoController.lassoSpawnLocation.position);
		lassoController.lassoLine.GetComponent<LineRenderer>().SetPosition(1, lineEndPos);
	}
}
