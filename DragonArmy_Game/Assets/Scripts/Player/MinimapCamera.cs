using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform playerTransform;

    // Update is called once per frame
    void Update()

    {
        transform.position = playerTransform.position + new Vector3(15, 10, 5);
    }

}
