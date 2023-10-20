using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity​Engine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument UIDocument;
    public GameObject toEnable;
    public PlayGamesServices playGamesServices;
    Button startButton, exitButton, clasificationButton;
    Label label;
    TextField textField;
    VisualElement root;
    void Start()
    {
        root = UIDocument.rootVisualElement;
        // get ui elements by name
        startButton = root.Q<Button>("StartButton");
        clasificationButton = root.Q<Button>("Clasification");
        exitButton = root.Q<Button>("ExitButton");

        // add event handler
        startButton.clickable.clicked += startApp;
        exitButton.clickable.clicked += exitApp;
        clasificationButton.clickable.clicked += showClasification;


    }



    private void startApp()
    {
        root.visible = false;
        toEnable.SetActive(true);
    }

    private void showClasification()
    {
        playGamesServices.showLeaderBoard();
    }

    private void exitApp()
    {
        #if UNITY_EDITOR
                // Application.Quit() does not work in the editor so
                // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }


}

