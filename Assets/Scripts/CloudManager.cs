using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CloudManager : MonoBehaviour
{

    public int totalClouds;
    public Terrain terrain;
    public Transform player;
    public Transform cloudCollider;
	public Transform cloudPrefab;
	public float colliderPullForce;
	
	[HideInInspector]
	public static List<CloudController> rainingClouds = new List<CloudController>();

    // Use this for initialization
    void Start()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;
        for (int i = 0; i < totalClouds; i++)
        {
            float xPos = Random.Range(terrainPosition.x + 200, terrainPosition.x - 200 + terrainSize.x);
            float zPos = Random.Range(terrainPosition.z + 200, terrainPosition.z - 200 + terrainSize.z);

            Vector3 randomPos = new Vector3(xPos, 150f, zPos);
            Transform cloud = (Transform)Instantiate(cloudPrefab, randomPos, Quaternion.identity);
			cloud.GetComponent<CloudController>().playerTransform = player;
		}
    }

    // Update is called once per frame
    void Update()
    {

    }
}
