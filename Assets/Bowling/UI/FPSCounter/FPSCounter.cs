using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity​Engine.UIElements;

public class FPSCounter : MonoBehaviour
{
    public UIDocument UIDocument;
    Label fpsViewer;
    float deltaTime;
    void Start()
    {
        var root = UIDocument.rootVisualElement;
        // get ui elements by name
        fpsViewer = root.Q<Label>("FPSViewer");

    }

    private void startApp()
    {
        gameObject.SetActive(false);
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



    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsViewer.text = Mathf.Ceil(fps).ToString();
    }
}

