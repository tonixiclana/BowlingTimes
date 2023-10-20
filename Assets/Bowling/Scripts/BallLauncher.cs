using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallLauncher : MonoBehaviour
{

    public Rigidbody ballObject;
    public GameObject pins;
    public Transform forcePosition;
    public Transform centerOfMass;

    public List<AudioClip> launchClips;

    public bool launched = false;

    private bool touchingLane = false;
    private float minArcComplete = 0.5f;
    public float actualVelocity = 0f;

    public int launchNumber = 0;
    public int numLaunchers = 0;

    public AudioClip clipSelected;




    private void Start()
    {
        if (ballObject == null)
            ballObject = this.GetComponent<Rigidbody>();

        if (centerOfMass != null)
            ballObject.centerOfMass = centerOfMass.transform.position;

    }

    public void launchBall(Vector3 force)
    {
        launched = true;

        if (forcePosition != null)
            ballObject.AddForceAtPosition(forcePosition.TransformDirection(force), forcePosition.transform.position, ForceMode.Impulse);
        else
            ballObject.AddForce(force, ForceMode.Impulse);


    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!touchingLane && collision.transform.CompareTag("Lane"))

        {
            touchingLane = true;

            clipSelected = launchClips[Random.Range(0, launchClips.Count)];
            Debug.Log("Entra de " + collision.transform.name);
            //ballObject.GetComponent<AudioSource>().volume = Utils.Map(minArcComplete, 0.5f, 1f, 0.4f, 1f);
            ballObject.GetComponent<AudioSource>().PlayOneShot(clipSelected);

        }
        

    }

    private void OnCollisionExit(Collision collision)
    {


        if (touchingLane && collision.transform.CompareTag("Lane"))
        {
            
            touchingLane = false;
            Debug.Log("Sale de " + collision.transform.name);
            //StartCoroutine(StopDelayed(ballObject.GetComponent<AudioSource>(), 0.1f));
            ballObject.GetComponent<AudioSource>().Stop();

        }
       

    }

    IEnumerator StopDelayed(AudioSource audioSource, float timeout = 4)
    {
        float volume = audioSource.volume;
        //Print the time of when the function is first called.
        for (int i = 0; i < 10; i++)
        {
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(timeout);
        }

        audioSource.Stop();
        audioSource.volume = volume;
    }
}
