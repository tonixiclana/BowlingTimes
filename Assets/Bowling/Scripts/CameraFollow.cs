using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using System.Threading.Tasks;

public class CameraFollow : MonoBehaviour
{
    public Transform targetObject;
    public Transform finishPoint;
    public float distanceToFinishForStop = 4f;
    private Vector3 initalOffset;
    private Vector3 cameraPosition;
    

    void Start()
    {

        initalOffset = transform.position - targetObject.position;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(targetObject.position, finishPoint.position) > distanceToFinishForStop)
        {
            cameraPosition = targetObject.position + initalOffset;
            transform.position = cameraPosition;
        }
    }

}