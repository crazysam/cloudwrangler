using UnityEngine;
using System.Collections;
using Holoville.HOTween;

public class LassoBar : MonoBehaviour
{
	public float objectScale = 0.0001f; 
	
	private Camera cam;
	private LassoController lasso;
    private Vector3 startingScale;
	
	// Use this for initialization
	void Start()
	{
		cam = transform.parent.GetComponentInChildren<Camera>();
		lasso = transform.parent.GetComponentInChildren<LassoController>();
		
		startingScale = transform.localScale;
		
		gameObject.SetActive(false);
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
		transform.localScale = Vector3.zero;
		HOTween.To(transform, 0.25f, "localScale", startingScale);
	}
	
	public void EndEngage(bool hideImmediately = true)
	{
		gameObject.SetActive(false);
	}
}
