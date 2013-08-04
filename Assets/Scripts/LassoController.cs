using UnityEngine;
using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;

public class LassoController : MonoBehaviour
{
	public int extendSpeed;
	public int minCloudColliderSize;
	public int cloudColliderShrinkSpeed;
	public float trailLifetime;
	public Transform cloudCollider;
	public Transform lassoSpawnLocation;
	public LayerMask raycastMask = -1;

	[HideInInspector]
	public bool isEngaged; // this is when lasso is extending (being drawn)
	[HideInInspector]
	public bool isHooked; // this is when cloud collider has grabbed some clouds (being pulled)
	
	private float lassoLifetime;
	private float colliderRadius;
	private List<Vector2> pointList;

	// Use this for initialization
	void Start()
	{
		HOTween.Init(false, false, true);
		HOTween.EnableOverwriteManager();
		DisengageLasso();
		UnhookLasso();
	}

	// Update is called once per frame
	void Update()
	{
		if (isEngaged)
		{
			lassoLifetime -= Time.deltaTime;
			if (lassoLifetime > 0)
			{
				RaycastHit hitInfo = new RaycastHit();
				Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(mouseRay, out hitInfo, float.MaxValue, raycastMask))
				{
					Vector3 worldMouseLocation = new Vector3(hitInfo.point.x, transform.position.y, hitInfo.point.z);
					
					pointList.Add(new Vector2(transform.position.x, transform.position.z)); // add old position
					transform.position = Vector3.MoveTowards(transform.position, worldMouseLocation, Time.deltaTime * extendSpeed);
					pointList.Add(new Vector2(transform.position.x, transform.position.z)); // add new position

					CheckLineIntersection();
				}
				else
					DisengageLasso();
			}
			else
				DisengageLasso();
		}
		else
			transform.position = lassoSpawnLocation.position;
		
		// shrink cloud collider
		if(cloudCollider.gameObject.activeSelf && colliderRadius != -1)
		{
			if(colliderRadius > 0 && CloudController.rainingClouds.Count > 0)
			{
				colliderRadius -= Time.deltaTime * cloudColliderShrinkSpeed;
				cloudCollider.localScale = new Vector3(colliderRadius, cloudCollider.localScale.y, colliderRadius);
			}
			else
			{
				UnhookLasso(true);
				cloudCollider.gameObject.SetActive(false);
			}
		}

		// Process Input
		if (!isEngaged && Input.GetMouseButtonDown(0))
		{
			EngageLasso();
		}
		else if (Input.GetMouseButtonUp(0))
		{
			DisengageLasso();
			UnhookLasso();
		}
	}

	private void EngageLasso()
	{
		isEngaged = true;
		lassoLifetime = trailLifetime;
		pointList = new List<Vector2>();
		transform.position = lassoSpawnLocation.position;
		GetComponent<TrailRenderer>().time = trailLifetime;
	}

	private void DisengageLasso()
	{
		isEngaged = false;
		GetComponent<TrailRenderer>().time = -1;
		cloudCollider.gameObject.SetActive(false);
	}
	
	void HookLasso()
	{
		isHooked = true;
	}
	
	void UnhookLasso(bool killClouds = false)
	{
		isHooked = false;
		
		foreach(CloudController cc in CloudController.rainingClouds)
		{
			if(killClouds)
				cc.SetDeadState();
			else
				cc.SetNormalState();
		}
		CloudController.rainingClouds = new List<CloudController>();
	}
	
	// lasso becomes hooked when there is clouds within cloud collider
	private void CheckLassoHooked()
	{
		if(CloudController.rainingClouds.Count > 0)
		{
			HookLasso();
		}
		else
		{
			UnhookLasso();
			// tween scale, so it scales out
			TweenParms tp = new TweenParms();
			tp.Prop("localScale", new Vector3(minCloudColliderSize, cloudCollider.localScale.y, minCloudColliderSize));
			tp.OnComplete( () => { cloudCollider.gameObject.SetActive(false); } );
			HOTween.To(cloudCollider, 0.25f, tp);
		}
	}

	private void CheckLineIntersection()
	{
		Vector2 newPointEndPos = pointList[pointList.Count - 1];
		Vector2 newPointStartPos = pointList[pointList.Count - 2];
		for (int i = 0; i < pointList.Count - 4; i += 2)
		{
			Vector2 prevPointStartPos = pointList[i];
			Vector2 prevPointEndPos = pointList[i + 1];
			if (FasterLineSegmentIntersection(newPointStartPos, newPointEndPos, prevPointStartPos, prevPointEndPos))
			{
				DisengageLasso();
				
				float maxDistance = 0;
				int mostDistantPoint = 0;
				for(int j = i; j < pointList.Count; j++)
				{
					float dist = Vector2.Distance(newPointEndPos, pointList[j]);
					if(dist > maxDistance)
					{
						maxDistance = dist;
						mostDistantPoint = j;
					}
				}
				
				float colliderRadius = maxDistance / 2;
				
				// make sure cloudCollider will be big enough
				if(colliderRadius > minCloudColliderSize)
				{
					this.colliderRadius = -1; // prevent update loop from prematurely killing Collider
					Vector2 midPoint = Vector2.Lerp(newPointEndPos, pointList[mostDistantPoint], 0.5f);
					
					// position and scale cloud collider
					cloudCollider.position = new Vector3(midPoint.x, cloudCollider.position.y, midPoint.y);
					
					// tween scale, so it bounces in
					TweenParms tp = new TweenParms();
					tp.Prop("localScale", new Vector3(colliderRadius, cloudCollider.localScale.y, colliderRadius));
					tp.Ease(EaseType.EaseOutElastic); // Bouncy!
					
					tp.OnComplete( () => { this.colliderRadius = colliderRadius; CheckLassoHooked(); } );
					HOTween.To(cloudCollider, 0.5f, tp);
					
					// activate object
					cloudCollider.gameObject.SetActive(true);
				}
				return;
			}
		}
	}

	// http://www.stefanbader.ch/faster-line-segment-intersection-for-unity3dc
	private static bool FasterLineSegmentIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
	{
		Vector2 a = p2 - p1;
		Vector2 b = p3 - p4;
		Vector2 c = p1 - p3;

		float alphaNumerator = b.y * c.x - b.x * c.y;
		float alphaDenominator = a.y * b.x - a.x * b.y;
		float betaNumerator = a.x * c.y - a.y * c.x;
		float betaDenominator = a.y * b.x - a.x * b.y;

		bool doIntersect = true;

		if (alphaDenominator == 0 || betaDenominator == 0)
		{
			doIntersect = false;
		}
		else
		{
			if (alphaDenominator > 0)
			{
				if (alphaNumerator < 0 || alphaNumerator > alphaDenominator)
				{
					doIntersect = false;
				}
			}
			else if (alphaNumerator > 0 || alphaNumerator < alphaDenominator)
			{
				doIntersect = false;
			}
			if (doIntersect && betaDenominator > 0)
			{
				if (betaNumerator < 0 || betaNumerator > betaDenominator)
				{
					doIntersect = false;
				}
			}
			else if (betaNumerator > 0 || betaNumerator < betaDenominator)
			{
				doIntersect = false;
			}
		}
		return doIntersect;
	}
}
