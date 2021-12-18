using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CameraController : Singleton<CameraController>
{
    public int cameraSettings = -1;
    public List<CameraSettings> settings;
    public bool won;
    [SerializeField]
    Transform player;
    [SerializeField]
    Vector3 cameraOffset;
    [SerializeField]
    float cameraCatchUpDistance = 2f;
    [SerializeField]
    float cameraSpeed = 1f;

    Vector3? cameraDestination;

    private void Start()
    {
        FocusOnPlayer();
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 destination = player.position + cameraOffset;
            if (Mathf.Abs(destination.z - transform.position.z) > cameraCatchUpDistance || Mathf.Abs(destination.x - transform.position.x) > cameraCatchUpDistance)
                cameraDestination = destination;
            //if (cameraDestination != null && (Mathf.Abs(destination.z - transform.position.z) < cameraCatchUpDistance / 2 || Mathf.Abs(destination.x - transform.position.x) < cameraCatchUpDistance / 2))
            //    cameraDestination = null;
            if (cameraDestination != null)
                transform.position = Vector3.Lerp(transform.position, cameraDestination.Value, cameraSpeed * Time.deltaTime);
        }
    }

    public void FocusOnPlayer()
    {
        if (cameraSettings > -1)
        {
            cameraOffset = settings[cameraSettings].offset;
            transform.eulerAngles = settings[cameraSettings].angle;
        }
        cameraDestination = null;
        transform.position = player.position + cameraOffset;
    }
}

[System.Serializable]
public class CameraSettings
{
    public Vector3 offset;
    public Vector3 angle;
}