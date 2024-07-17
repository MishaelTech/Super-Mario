using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiteScrolling : MonoBehaviour
{
    private Transform player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    private void LateUpdate()
    {
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = Mathf.Max(cameraPosition.x, cameraPosition.y, player.position.x, player.position.y);
        transform.position = cameraPosition;
    }
}
