using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PinManager pinManager;
    public GameObject scorePrefab;
    public TextMeshProUGUI playerNameText;
    public RectTransform ScoreTab;
    public string playerName;
    //public PlayGamesServices playGamesServices;

    private List<GameObject> scoresGameObject = new List<GameObject>(10);
    // Start is called before the first frame update
    void Start()
    {
        playerNameText.text = playerName;

        scoresGameObject.Add(Instantiate(scorePrefab, ScoreTab));
    }


    void Update()
    {



        if (scoresGameObject.Count < pinManager.scores.Count)
        {
            scoresGameObject.Add(Instantiate(scorePrefab, ScoreTab));
        }
        else
        if(pinManager.scores.Count > 0)
        {
            int counter = 0;
            foreach(Vector3 score in pinManager.scores)
            {
                if(score.z == PinManager.STRIKE)
                {
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[0].text = "";
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text = "X";
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[2].text = "";
                }
                else
                if(score.z == PinManager.SPARE)
                {
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[0].text = score.x.ToString();
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text = "/";
                    scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[2].text = "";
                }
                else
                {
                    if (scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text != "X")
                        scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[0].text = score.x.ToString();

                    if (score.y == PinManager.PENDING)
                        scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text = "";
                    else
                        if(scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text != "/" 
                        && scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text != "X")
                            scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[1].text = score.y.ToString();

                    if(counter == 0)
                        scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[2].text = score.z.ToString();
                    else
                    {
                        int sum = 0;
                        if (int.TryParse(scoresGameObject[counter - 1].GetComponentsInChildren<TextMeshProUGUI>()[2].text, out sum))
                            scoresGameObject[counter].GetComponentsInChildren<TextMeshProUGUI>()[2].text = (sum + score.z).ToString();
                    }
                   

                }

                counter += 1;
            }

        }


        

        if (scoresGameObject.Count > pinManager.scores.Count + 1)
        {
            //playGamesServices.postScore(long.Parse(scoresGameObject[scoresGameObject.Count - 1].GetComponentsInChildren<TextMeshProUGUI>()[2].text));
            removeResults();
        
            scoresGameObject.Add(Instantiate(scorePrefab, ScoreTab));
        }
    }

    void removeResults()
    {
        foreach (GameObject gm in scoresGameObject)
            Destroy(gm);
        scoresGameObject.Clear();
    }
}
