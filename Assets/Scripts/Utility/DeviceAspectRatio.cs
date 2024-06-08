using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceAspectRatio : MonoBehaviour
{
    private float aspectRatio;
    private Camera camera;
    public float aspectRatio16x9;
    public float aspectRatio18x9;
    public float aspectRatio20x9;
    public float aspectRatioBigger;
    void Awake()
    {
        aspectRatio = (float)Screen.height / Screen.width;
        camera = GetComponent<Camera>();

        if((float)Screen.height > (float)Screen.width)
        {
            PerformActionBasedOnAspectRatio();
        }
    }

    void PerformActionBasedOnAspectRatio()
    {
        if (Mathf.Approximately(aspectRatio, 16f / 9f))
        {
            camera.orthographicSize = aspectRatio16x9;
        }
        else if (Mathf.Approximately(aspectRatio, 18f / 9f))
        {
            camera.orthographicSize = aspectRatio18x9;
        }
        else if (Mathf.Approximately(aspectRatio, 20f / 9f))
        {
            camera.orthographicSize = aspectRatio20x9;
        }
        else
        {
            Debug.Log("Aspect Ratio is " + aspectRatio);
            camera.orthographicSize = aspectRatioBigger;
        }
    }
}
