using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    public AudioClip fallPinClip;
    public AudioClip touchesPinClip;
    public float frecuencySound = 1f;
    public bool pinFall = false;
    public Transform centerOfMass;



    Vector3 initPosition;
    float lastSoundPlayedTime;
    // Start is called before the first frame update
    void Start()
    {
        initPosition = transform.position;
        lastSoundPlayedTime = Time.fixedTime;
        if (centerOfMass)
            GetComponent<Rigidbody>().centerOfMass = centerOfMass.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.transform.CompareTag("Lane") || collision.transform.CompareTag("Ball")) 
            && initPosition != transform.position 
            && collision.impulse.magnitude > 0.35f)
        {
            initPosition = transform.position;
            lastSoundPlayedTime = Time.fixedTime;
            Debug.Log(name + " Choca con " + collision.transform.name + " a " + collision.impulse.magnitude);
            //ballObject.GetComponent<AudioSource>().volume = map(minArcComplete, 0.5f, 1f, 0f, 1.1f);
            GetComponent<AudioSource>().PlayOneShot(touchesPinClip, Utils.Map(collision.impulse.magnitude, 0f, 10f, 0.3f, 1f));
        }
        if (pinFall && collision.transform.CompareTag("Ball") && collision.impulse.magnitude > 0f)
        {
            pinFall = false;
            Debug.Log(name + " Choca con " + collision.transform.name + " a " + collision.impulse.magnitude);
            GetComponent<AudioSource>().PlayOneShot(fallPinClip, Utils.Map(collision.impulse.magnitude, 0f, 10f, 0.3f, 1f));
        }

    }

}
