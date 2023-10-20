using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public Rigidbody ballObject;
    public GameObject pins;
    public float minForce = 10f;
    public float maxForce = 40f;
    public float maxTorque = 1;
    public Transform maxArcPoint;
    public Transform centerArcPoint;
    public Transform minArcPoint;
    public Transform maxLeftPoint;
    public Transform maxRightPoint;



    private Vector2 startPos = Vector2.zero;
    private Vector2 direction = Vector2.zero;
    private Vector2 forzeStartPos = Vector2.zero;
    private Vector2 forzeDirection = Vector2.zero;
    private Vector2 position = Vector2.zero;
    private bool touchingLane = false;
    public float minArcComplete = 0.5f;
    public float arcComplete = 0.5f;
    public float previousArcComplete = 0.5f;
    public float horizontalComplete = 0.5f;
    public float actualVelocity = 0f;

    public int launchNumber = 0;
    public int numLaunchers = 0;

    bool directionChosen = false;
    private bool takenBall = false;


    private Vector3 initPosition;
    private Quaternion initRotation;
    private GameObject _pins;
    private float lastTouchDirection = 0f;
    private Touch touch;
    private Ray ray;
    private float torqueComplete = 0f;
    private Vector3 previousPosition = Vector3.zero;
    private AudioClip clipSelected;




    private void Start()
    { 
        if (!ballObject)
            Debug.Log("Error with BallController: BallObject needed");

        previousPosition = ballObject.position;
        ballObject.maxAngularVelocity = float.MaxValue;
        torqueComplete = maxTorque;

        
        ballObject.isKinematic = true;



        initPosition = ballObject.transform.position;
        initRotation = ballObject.transform.rotation;
        // _pins = Instantiate(pins);

        Debug.DrawRay(ballObject.position, ballObject.transform.forward * 100, Color.red, 10);
        if (Input.touchCount == 1)
            touch = Input.GetTouch(0);
        else
        {

            touch = new Touch();

        }
    }


    private void FixedUpdate()
    {
        //if (launched)
        //{
        //    ballObject.AddTorque(new Vector3(torqueComplete, 0, 0), ForceMode.Force);
        //}
        if (InputHelper.GetTouches().Count > 0)
            touch = InputHelper.GetTouches()[0];

            //if(directionChosen && launched)
            //{
            //    ballObject.AddTorque(new Vector3(0, dir));
            //}

            if (directionChosen && !ballObject.GetComponent<BallLauncher>().launched)
            {
                Vector3 touchDirection = new Vector3(-direction.x, 0, -direction.y);
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(new Vector3(-position.x, 0, -position.y));
                // print("startPos: " + startPos + " direction: " + actualPos + " magnitude: " + worldDirection.magnitude / 15);
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1.5f);
                //ballObject.transform.rotation = Quaternion.LookRotation(worldDirection, Vector3.forward);
                //Debug.Log(touchDirection + " -- " + touchPosition);
                //startPoint.position += (new Vector3(actualPos.x, 0, 0));
                //endPoint.position += (new Vector3(actualPos.x, 0, 0));




                if (takenBall && (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Began))
                {
                    arcComplete = Utils.Map(touchDirection.z, -180f, 180f, 0f, 1f);
                    if (arcComplete > 1f)
                        arcComplete = 1f;
                    else
                    if (arcComplete < 0f)
                        arcComplete = 0f;

                    horizontalComplete = Utils.Map(touchDirection.x, 360, -360f, 0f, 1f);
                    if (lastTouchDirection < 0.5f)
                        horizontalComplete -= 0.5f - lastTouchDirection;
                    else
                        horizontalComplete += lastTouchDirection - 0.5f;


                    if (horizontalComplete > 1f)
                        horizontalComplete = 1f;
                    else
                    if (horizontalComplete < 0f)
                        horizontalComplete = 0f;

                    if (minArcComplete < arcComplete & arcComplete > 0.5f)
                        minArcComplete = arcComplete;

                    if (arcComplete < 0.5f && 0.5f - arcComplete >= minArcComplete - 0.5f)
                        arcComplete = 0.5f - (minArcComplete - 0.5f);

                    Vector3 arcCenter = (maxArcPoint.position + minArcPoint.position) * 0.5F;
                    //set arc radius
                    arcCenter -= new Vector3(0, -0.1f, 0);
                    //set relative center
                    Vector3 maxArcRelCenter = maxArcPoint.position - arcCenter;
                    Vector3 minArcRelCenter = minArcPoint.position - arcCenter;
                    //Debug.Log("fracComplete: " + fracComplete);
                    //Debug.Log("Launch force: " + Vector3.forward * map(minFracComplete, 0.5f, 1f, 0f, 30f));
                    maxArcPoint.position = new Vector3(Vector3.Lerp(maxLeftPoint.position, maxRightPoint.position, horizontalComplete).x, maxArcPoint.position.y, maxArcPoint.position.z);
                    minArcPoint.position = new Vector3(Vector3.Lerp(maxLeftPoint.position, maxRightPoint.position, horizontalComplete).x, minArcPoint.position.y, minArcPoint.position.z);
                    centerArcPoint.position = new Vector3(Vector3.Lerp(maxLeftPoint.position, maxRightPoint.position, horizontalComplete).x, centerArcPoint.position.y, centerArcPoint.position.z);
                    ballObject.position = Vector3.Slerp(maxArcRelCenter, minArcRelCenter, arcComplete);
                    ballObject.position += arcCenter;
                }
                //launchBall(worldDirection.normalized * (worldDirection.magnitude / 15));
            }
    }

    public void resetBall()
    {
        ballObject.isKinematic = true;

        torqueComplete = 0f;
        ballObject.transform.position = centerArcPoint.position;
        ballObject.transform.rotation = initRotation;
        ballObject.GetComponent<BallLauncher>().launched = false;
        lastTouchDirection = 0.5f;
        minArcComplete = 0.5f;
        arcComplete = 0.5f;
        clipSelected = null;
    }

    // Update is called once per frame
    void Update()
    {
        actualVelocity = ballObject.velocity.sqrMagnitude;

        previousPosition = ballObject.position;

        if (InputHelper.GetTouches().Count > 0)
        { 
            touch = InputHelper.GetTouches()[0];
            ray = Camera.main.ScreenPointToRay(touch.position);

            if (!takenBall)
                takeBall(ray);
        }


        if (!ballObject.GetComponent<BallLauncher>().launched && takenBall && InputHelper.GetTouches().Count > 0)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    position = startPos;
                    directionChosen = false;
                    ballObject.isKinematic = true;
                    lastTouchDirection = horizontalComplete;
                    break;

                case TouchPhase.Moved:
                    direction = touch.position - startPos;
                    position = touch.position;
                    directionChosen = true;
                    if (previousArcComplete > arcComplete)
                        forzeDirection = touch.position - forzeStartPos;
                    else
                        forzeStartPos = touch.position;

                    previousArcComplete = arcComplete;

                    break;

                // Report that a direction has been chosen when the finger is lifted.
                case TouchPhase.Ended:
                    direction = touch.position - startPos;
                    directionChosen = true;
                    position = touch.position;

                    if (arcComplete < 0.5f && minArcComplete >= 0.5)
                    {
                        ballObject.isKinematic = false;
                        Debug.Log("Launch force: " + Vector3.forward * Utils.Map(minArcComplete, 0.4f, 1f, minForce, maxForce));
                        ballObject.GetComponent<BallLauncher>().launchBall(new Vector3(Utils.Map(forzeDirection.x, -200f, 200f, -5, 5), 0, Utils.Map(minArcComplete, 0.5f, 1f, minForce, maxForce)));
                        takenBall = false;


                    }
                    else
                    {
                        ballObject.position = centerArcPoint.position;
                        ballObject.rotation = initRotation;
                        minArcComplete = 0.5f;
                        arcComplete = 0.5f;
                        direction = Vector2.zero;
                        takenBall = false;
                    }

                    break;
            }


        }




    }

    //private void FixedUpdate()
    //{
    //if (_torque != vector3.zero)
    //{
    //    ballobject.addrelativetorque(_torque, forcemode.force);
    //    if (_torque.magnitude > 0f)
    //    {
    //        _torque -= _torque.normalized * torquedecrement;
    //        if (_torque.magnitude < 0f)
    //            _torque = vector3.zero;
    //    }
    //    else
    //    {
    //        _torque += _torque.normalized * torquedecrement;
    //        if (_torque.magnitude > 0f)
    //            _torque = vector3.zero;
    //    }

    //}

    // }

    void takeBall(Ray ray)
    {
        if (InputHelper.GetTouches().Count > 0)
            touch = InputHelper.GetTouches()[0];
        if (InputHelper.GetTouches().Count > 0 && InputHelper.GetTouches()[0].phase == TouchPhase.Began)
        {
            for (var i = 0; i < InputHelper.GetTouches().Count; ++i)
            {

                RaycastHit hitInfo;
                Physics.Raycast(ray, out hitInfo);

                Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1f);

                if (hitInfo.transform != null && hitInfo.transform.CompareTag("Ball") 
                    && Camera.main.GetComponent<CameraFollow>().targetObject == hitInfo.transform)
                    takenBall = true;

            }


        }
    }



}
