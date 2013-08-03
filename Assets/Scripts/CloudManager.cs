using UnityEngine;
using System.Collections;

public class CloudManager : MonoBehaviour
{

    public int total_clouds;
    public Transform cloud;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < total_clouds; i++)
        {
            float xPos = Random.Range(0, 10);
            float zPos = Random.Range(0, 10);

            Vector3 randomPos = new Vector3(xPos, 1.5f, zPos);
            Instantiate(cloud, randomPos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
