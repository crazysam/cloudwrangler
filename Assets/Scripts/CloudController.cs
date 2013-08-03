using UnityEngine;
using System.Collections;

public class CloudController : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * .05f * Time.deltaTime);
        transform.Translate(Vector3.forward * .05f * Time.deltaTime);
    }
}
