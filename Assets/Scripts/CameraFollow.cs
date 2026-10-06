using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 wantedPosition = target.position + offset;

        if (smoothTime <= 0f)
        {
            transform.position = wantedPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                wantedPosition,
                ref velocity,
                smoothTime
            );
        }
    }
}
