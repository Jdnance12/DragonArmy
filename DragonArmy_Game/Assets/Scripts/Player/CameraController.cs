using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerPos;
    [SerializeField] float followSpeed;

    [SerializeField] LayerMask wallLayer;
    [SerializeField] float transparentAlpha = 0.3f;

    public Renderer wallRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
        WallTransparency();
    }

    void FollowPlayer()
    {
        transform.position = playerPos.position;
    }
    void WallTransparency()
    {
        RaycastHit hit;
        Vector3 direction = playerPos.position - Camera.main.transform.position;
        Debug.DrawRay(Camera.main.transform.position, direction, Color.red);

        if (Physics.Raycast(Camera.main.transform.position, direction, out hit, direction.magnitude, wallLayer))
        {
            // Get the Renderer of the wall and set the transparency
            wallRenderer = hit.collider.GetComponent<Renderer>();
            if (wallRenderer != null)
            {
                Color color = wallRenderer.material.color;
                color.a = transparentAlpha;
                wallRenderer.material.color = color;
            }            
        }
        else
        {
            if (wallRenderer != null)
            {
                Color color = wallRenderer.material.color;
                color.a = 1f; // reset the transparency to fully opaque
                wallRenderer.material.color = color;
                wallRenderer = null;
            }
        }
    }
}
