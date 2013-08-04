using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour
{

    public int totalClouds;
    public Terrain terrain;
    public Transform cloud;
    public Transform player;
    public Transform cloudCollider;
    public float runThresholdLower;
    public float runThresholdUpper;
    public float runVelocityLower;
    public float runVelocityUpper;
    public float normalVelocityLower;
    public float normalVelocityUpper;

    // Use this for initialization
    void Start()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPosition = terrain.transform.position;
        for (int i = 0; i < totalClouds; i++)
        {
            float xPos = Random.Range(terrainPosition.x + 200, terrainPosition.x - 200 + terrainSize.x);
            float zPos = Random.Range(terrainPosition.z + 200, terrainPosition.z - 200   + terrainSize.z);

            Vector3 randomPos = new Vector3(xPos, 150f, zPos);
            Transform newCloud = (Transform)Instantiate(cloud, randomPos, Quaternion.identity);
            CloudController ctrl = newCloud.GetComponent<CloudController>();
            ctrl.playerTransform = player;
            ctrl.runThreshold = Random.Range(runThresholdLower, runThresholdUpper);
            ctrl.runVelocity = Random.Range(runVelocityLower, runVelocityUpper);
            ctrl.normalVelocity = Random.Range(normalVelocityLower, normalVelocityUpper);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
