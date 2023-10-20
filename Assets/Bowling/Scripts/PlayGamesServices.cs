using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class PlayGamesServices : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] InputField leaderBoard;
    public GameController gameController;

    public string userName = "";

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    //Iniciamos en Google Play



    void Initialize()
    {
        debugText.text = "PlayGame Init";
        sign();
    }

    void sign()
    {
        debugText.text = "En Play Services";
        try
        {

        #if UNITY_ANDROID
                    debugText.text = "En Android";
                    PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
                    PlayGamesPlatform.InitializeInstance(config);
                    PlayGamesPlatform.DebugLogEnabled = true;
                    PlayGamesPlatform.Activate();
                    PlayGamesPlatform.Instance.localUser.Authenticate((bool success) =>
                    {
                        if (success)
                        {
                            userName = PlayGamesPlatform.Instance.localUser.userName;
                            gameController.playerNameText.text = userName;
                        }
                            
                        debugText.text = success + " :" + PlayGamesPlatform.Instance.localUser.userName;

                    });



        #endif
        }
        catch (Exception exception)
        {
            debugText.text = "Exception " + exception;
        }
    }

    public void postScore(long score)
    {

        
    }

    public void showLeaderBoard()
    {

    }

    public void achievementCompleted()
    {
        //Social.ReportProgress("")
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
