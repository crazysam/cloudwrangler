using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class LassoBar : MonoBehaviour
{
	private Camera cam;
	private LassoController lasso;
	
	// Use this for initialization
	void Start()
	{
		cam = transform.parent.GetComponentInChildren<Camera>();
		lasso = transform.parent.GetComponentInChildren<LassoController>();
	}
	
	// Update is called once per frame
	void Update()
	{
		// make object face camera (billboard)
		transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
		
		if(lasso.isEngaged)
		{
			renderer.material.SetFloat("_Cutoff", (lasso.trailLifetime - lasso.lassoLifetime) / lasso.trailLifetime); 
		}
	}
	
	public void BeginEngage()
	{
		gameObject.SetActive(true);
		renderer.material.SetFloat("_Cutoff", 0);
	}
	
	public void EndEngage()
	{
		gameObject.SetActive(false);
	}
}
