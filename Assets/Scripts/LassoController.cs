using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LassoController : MonoBehaviour
{
	public int moveSpeed;
	public float trailLifetime;
	public Transform cloudCollider;
	public Transform lassoSpawnLocation;

	[HideInInspector]
	public bool isEngaged;
	private float lifetimeCounter;
	private List<Vector2> pointList;

	// Use this for initialization
	void Start()
	{
		DisengageLasso();
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
					
					pointList.Add(new Vector2(transform.position.x, transform.position.z)); // add old position
					transform.position = Vector3.MoveTowards(transform.position, worldMouseLocation, Time.deltaTime * moveSpeed);
					pointList.Add(new Vector2(transform.position.x, transform.position.z)); // add new position

					CheckLineIntersection();
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
		pointList = new List<Vector2>();
		GetComponent<TrailRenderer>().time = trailLifetime;
		transform.position = lassoSpawnLocation.position;
	}

	private void DisengageLasso()
	{
		isEngaged = false;
		GetComponent<TrailRenderer>().time = 0;
		cloudCollider.gameObject.SetActive(false);
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
				print("INTERSECTION!!!");
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
				
				Vector2 midPoint = Vector2.Lerp(newPointEndPos, pointList[mostDistantPoint], 0.5f);
				
				// position and scale cloud collider
				cloudCollider.position = new Vector3(midPoint.x, cloudCollider.position.y, midPoint.y);
				cloudCollider.localScale = new Vector3(maxDistance / 2, cloudCollider.localScale.y, maxDistance / 2);
				cloudCollider.gameObject.SetActive(true);
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
