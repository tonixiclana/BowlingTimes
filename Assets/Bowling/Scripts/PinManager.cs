using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinManager : MonoBehaviour
{
    public const float STRIKE = -1f;
    public const float SPARE = -2f;
    public const float PENDING = -3f;


    public GameObject pinPrefab;
    public BallController ballController;
    public List<Transform> pinsPosition = new List<Transform>(10);
    public int numberOfThrows = 2;
    public int numberOfturns = 10;
    public GameObject ball;
    public List<string> standPins;
    public List<Vector3> scores = new List<Vector3>(10);


    public List<string> pinsDrop = new List<string>(10);
    public List<GameObject> pins = new List<GameObject>(10);


    public int actualThrow;
    public int actualTurn;

    private bool throwStart;
    private Rigidbody rb;
    private Collider groundCollider;




    // Start is called before the first frame update
    void Start()
    {
        throwStart = false;
        actualThrow = 0;
        actualTurn = 0;
        rb = GetComponent<Rigidbody>();
        groundCollider = GetComponentInChildren<Collider>();
        if(pinsPosition.Count >= 1)
        {
            foreach(Transform pinPosition in pinsPosition)
            {
                GameObject gm = Instantiate(pinPrefab, pinPosition);
                gm.name = pinPosition.name;
                standPins.Add(gm.name);
                pins.Add(gm);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.transform.name == "Ball")
        {
            foreach (GameObject gm in pins)
                gm.GetComponent<Rigidbody>().isKinematic = false;
            ball = other.gameObject;
            if (!throwStart)
            {
                throwStart = true;
                StartCoroutine(ExampleCoroutine(other.transform.name));
            }

        }
        
    }


    void OnTriggerExit(Collider other)
    {
        if (other.transform.name == "sensor")
        {
            Debug.Log("Entra en zona un sensor" + other.transform.parent.name);
            if (!pinsDrop.Contains(other.transform.parent.name))
            {
                standPins.Remove(other.transform.parent.name);
                pinsDrop.Add(other.transform.parent.name);
                other.transform.parent.GetComponent<Pin>().pinFall = true;
            } 
        }

        //foreach (ContactPoint contact in collision.)
        //{
        //    print(contact.point);
        //    Debug.DrawRay(contact.point, contact.normal, Color.white);
        //}
    }

    void cleanDroppedPins()
    {
        List<GameObject> pinsToDelete = new List<GameObject>(10);
        List<Transform> pinsToAddPosition = new List<Transform>(10);

        for (int i = 0; i < pins.Count; i++)
        {
            if (standPins.Contains(pins[i].name))
            {
                Transform pinParent = pins[i].transform.parent;
                //pins.Remove(pins[i]);
                pinsToAddPosition.Add(pinParent);

            }
            else
            {
                pinsToDelete.Add(pins[i]);
            }
        }

        //Se borra todos los bolos
        deleteAllPins();

        //Se añaden los que quedaron de pie
        if (pinsToAddPosition.Count <= 0)
            putPins();

        foreach (Transform pTA in pinsToAddPosition)
        {
            string pinName = pTA.name;

            GameObject pin = Instantiate(pinPrefab, pTA);
            pins.Add(pin);
            pin.name = pinName;
        }

       

        throwStart = false;
    }


    void deleteAllPins()
    {
        foreach (GameObject pTA in pins)
        {
            Destroy(pTA);
            //pins.Remove(pins[count]);
        }
        pins.Clear();
    }
    void putPins()
    {
        standPins.Clear();
        pinsDrop.Clear();
        deleteAllPins();
        for (int i = 0; i < pinsPosition.Count; i++)
        {
            Transform pinParent = pinsPosition[i];
            GameObject pin = Instantiate(pinPrefab, pinParent);
            pin.name = pinParent.name;
            pins.Add(pin);
            standPins.Add(pin.name);
        }

        throwStart = false;

    }

    IEnumerator ExampleCoroutine(string name, float timeout = 4)
    {
        //Print the time of when the function is first called.
        Debug.Log("Started Coroutine at timestamp : " + Time.time + " " + name);



        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(timeout);

        actualThrow += 1;

        if(actualThrow < numberOfThrows && standPins.Count == 0)
        {
            scores.Add(new Vector3(pinsDrop.Count, 0, STRIKE));
            actualThrow = 0;
            actualTurn += 1;
            putPins();
        }
        else if(actualThrow >= numberOfThrows)
        {

            
            if (scores[actualTurn].x + pinsDrop.Count - scores[actualTurn].x >= pinsPosition.Count)
            {
                scores[actualTurn] = new Vector3(scores[actualTurn].x, pinsDrop.Count - scores[actualTurn].x, SPARE);
            }
            else
            {
                scores[actualTurn] = new Vector3(scores[actualTurn].x, pinsDrop.Count - scores[actualTurn].x);

            }
            putPins();
            actualTurn += 1;
            actualThrow = 0;
        }
        else
        {
            scores.Add(new Vector3(pinsDrop.Count, PENDING));
            cleanDroppedPins();
        }

        if(scores.Count > 0)
            recalculateRecursive(scores, scores.Count);

        if (actualTurn >= numberOfturns)
        {
            yield return new WaitForSeconds(5f);
            actualTurn = 0;
            scores.Clear();

        }


        yield return new WaitForSeconds(timeout / timeout);

        //After we have waited 5 seconds print the time again.
        Debug.Log("Finished Coroutine at timestamp : " + Time.time);
        ballController.resetBall();
    }

    void recalculateRecursive(List<Vector3> scores, int position)
    {
        if(scores.Count > 0 && position > 0)
        {
            Vector3 score;
            int relPos = position - 1;

            score = scores[relPos];

            if (relPos < 0)
            {

            }
            else
            {
                if(score.z > 0 && actualTurn != 1)
                {

                }
                else
                if (score.y == PENDING)
                {
                    score.z = (int)score.x;
                    scores[relPos] = score;
                    recalculateRecursive(scores, position - 1);
                }
                else 
                if (score.z != SPARE && score.z != STRIKE)
                {
                    score.z = (int)score.x + (int)score.y;

                    scores[relPos] = score;

                    recalculateRecursive(scores, position - 1);
                }
                else
                if (score.z == SPARE)
                {
                    if (position < scores.Count && scores.Count >= 2 && scores[position].z != SPARE && scores[position].z != STRIKE)
                    {
                        score.z = 0;
                        score.z += scores[position].x;
                        score.z += pinsPosition.Count;
                        scores[relPos] = score;
                    }
                    else
                    if(scores.Count >= numberOfturns)
                    {
                        score.z = 0;
                        score.z += pinsPosition.Count;
                        scores[relPos] = score;
                    }

                    recalculateRecursive(scores, position - 1);
                }
                else 
                if (score.z == STRIKE)
                {
                    if (position < scores.Count && scores.Count >= 2 && scores[position].z != SPARE && scores[position].z != STRIKE && scores[position].y != PENDING)
                    {
                        score.z = scores[position].z;
                        if(score.z < pinsPosition.Count * 3f)
                            score.z += pinsPosition.Count;
                        else
                            score.z += pinsPosition.Count * 3f;

                        scores[relPos] = score;

                    }
                    else
                    if (scores.Count >= numberOfturns)
                    {
                        score.z = pinsPosition.Count;
                        scores[relPos] = score;
                    }

                    recalculateRecursive(scores, position - 1);
                }
            }
        }
        
    }
}
