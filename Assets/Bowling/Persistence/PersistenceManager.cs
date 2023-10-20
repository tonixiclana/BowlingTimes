﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PersistenceManager : MonoBehaviour {


     public List<PersistenceGameobject> persistenceGameobjects;
    private List<GameObject> gmL = new List<GameObject>();

    /// <summary>
    /// TMP_InputField to write the name of savegame to store
    /// </summary>
    [Tooltip("TMP_InputField to write the name of savegame to store")]
    public string _saveGameName;

    /// <summary>
    /// TMP_InputField to write the description of Savegame to store
    /// </summary>
    [Tooltip("TMP_InputField to write the description of Savegame to store")]
    public string _saveGameDescription;



    // Use this for initialization
    void Start () {

       

        //Add listener to savegame button
        //_savegameButton.onClick.AddListener(delegate
        //{
        //    if (_fileBrowser.existInActualPath(_nameOfSavegameInput.text + ".txt"))
        //        _fileBrowser.confirmMessage.showConfirmMessage("Do You want Overwrite this scenary?", saveScenary);
        //    else
        //    {
        //        saveScenary();
        //    }

        //});

        ////Add listener to removegame button
        //_removegameButton.onClick.AddListener(delegate
        //{
        //    try
        //    {
        //         if (_fileBrowser.hasContent(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VirtualADLSimulator/" + _nameOfSavegameInput.text))
        //            _fileBrowser.confirmMessage.showConfirmMessage("Do you want delete the experiments associated?", removeExperimentsAssociated);
        //    }
        //    catch(Exception e)
        //    {
        //        _fileBrowser.confirmMessage.showMessage("Error removing: " + e.Message);
        //    }
        //    //checking if folder in documents for experiments exists
           

        //    if (_fileBrowser.existInActualPath(_nameOfSavegameInput.text + ".txt"))
        //            _fileBrowser.confirmMessage.showConfirmMessage("Do You want delete this scenary?", removeScenary);
            
            
        //});


        ////Add listener to removegame button
        //_loadgameButton.onClick.AddListener(delegate
        //{
        //    _fileBrowser.confirmMessage.showConfirmMessage("Do You want load this scenary?, The changes unload will be lost", loadScenary);
        //    //loadPersistenceGameobjects(_nameOfSavegameInput.text + ".txt");
           
        //    //loadPersistenceGameobjects("savegames/" + _nameOfSavegameInput.text + ".txt");
        //});

        //_nameOfSavegameInput.onValueChanged.AddListener(delegate 
        //{
        //    updateActionsButtons();
        //});

        //_descriptionOfSavegameInput.onValueChanged.AddListener(delegate
        //{
        //    updateActionsButtons();
        //});

        //_fileBrowser.refreshGridContentOfActualPath();
        //updateFileBrowserItemListeners();


    }
	
    public void updateFileBrowserItemListeners()
    {
        //foreach (FileBrowserItem item in _fileBrowser.getFileItems())
        //{
            
        //    item.GetComponent<Button>().onClick.AddListener(delegate
        //    {
        //        _nameOfSavegameInput.text = JsonUtility.FromJson<SavegameInfo>(_fileBrowser.readFileInActualPath(item.GetComponentInChildren<TextMeshProUGUI>().text)[0]).name;
        //        _descriptionOfSavegameInput.text = JsonUtility.FromJson<SavegameInfo>(_fileBrowser.readFileInActualPath(item.GetComponentInChildren<TextMeshProUGUI>().text)[0]).description;
        //        _loadgameButton.interactable = true;
        //        _removegameButton.interactable = true;

        //    });

            
        //}

        //updateActionsButtons();
    }

	// Update is called once per frame
	void FixedUpdate () {
        //persistenceGameobjects = GameObject.FindObjectsOfType<PersistenceGameobject>().Cast<PersistenceGameobject>().ToList();
	}

    //Update the actions button to ensure the integrity of actions depending of value of inputs
    public void updateActionsButtons()
    {
        //_loadgameButton.interactable = false;
        //_removegameButton.interactable = false;

        //if (_nameOfSavegameInput.text == "" || _descriptionOfSavegameInput.text == "")
        //    _savegameButton.interactable = false;
        //else
        //{
        //    _savegameButton.interactable = true;

        //    foreach (FileBrowserItem item in _fileBrowser.getFileItems())
        //    {
        //        if(item.absolutePath.Split('\\').Last().Split('.')[0] == _nameOfSavegameInput.text)
        //        {
        //            _loadgameButton.interactable = true;
        //            _removegameButton.interactable = true;
        //            break;
        //        }

        //    }
        //}
           
    }

    public List<PersistenceGameobject> getPersistenceGameobjects()
    {
        persistenceGameobjects.Clear();

        foreach(var gm in gmL)
        {
            persistenceGameobjects.Add(gm.GetComponent<PersistenceGameobject>());
        }

        
        persistenceGameobjects.Sort(
            delegate(PersistenceGameobject c1, PersistenceGameobject c2) 
            {
                return c1.id.CompareTo(c2.id);
            }
        );
        
        return persistenceGameobjects;
    }

    public void saveGameObjectsInJSON(string path)
    {
        gmL.Clear();

        foreach(var gm in FindObjectsOfType<PersistenceGameobject>())
        {
            gmL.Add(gm.gameObject);
        }
   

        foreach (PersistenceGameobject persistanceGameObject in getPersistenceGameobjects())
        {
            persistanceGameObject.updatePersistenceInfo();
            //var json = JsonUtility.ToJson(persistanceGameObject.persistenceInfo, true);

           

            var json = JsonConvert.SerializeObject(persistanceGameObject.persistenceInfo, Formatting.Indented);


            //Debug.Log(json);


            //Write some text to the test.txt file
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine("Test");
            writer.Close();

            //Re-import the file to update the reference in the editor
            //AssetDatabase.ImportAsset(path);
            TextAsset asset = (TextAsset)Resources.Load("test");
            //_fileBrowser.editFileInActualPath(json, path);
        }

    }


    public void resetScene()
    {
        foreach (var pGm in gmL)
            Destroy(pGm);

        gmL.Clear();
        PersistenceGameobject.ResetIdCounter();
        if (persistenceGameobjects != null)
            persistenceGameobjects.Clear();
    }

    public void loadPersistenceGameobjects(string path)
    {
        var appPath = Application.persistentDataPath + "/" + path;
        string[] file = File.ReadAllLines(appPath);

        resetScene();


        SavegameInfo savegameInfo = JsonConvert.DeserializeObject<SavegameInfo>(file[0]);
        Debug.Log(savegameInfo.savegameTime);
        //FindObjectOfType<DayNight>().setActualDateTime(DateTime.ParseExact(savegameInfo.savegameTime, "yyyy-MM-dd HH:mm:ss", null));

        foreach (var line in file)
        {
            if(line != "")
            {


                PersistenceInfo persistenceInfo = JsonConvert.DeserializeObject<PersistenceInfo>(line);

                

                //PersistenceInfo persistenceInfo = JsonUtility.FromJson<PersistenceInfo>(line);
                if (persistenceInfo.id != -1)
                {

                    GameObject gm = new GameObject();
                    gm.AddComponent<PersistenceGameobject>().persistenceInfo = persistenceInfo;
                    gm.GetComponent<PersistenceGameobject>().loaded = true;
                    gmL.Add(gm);

                }
            }
 

        }

        int i = 0;
        foreach (var line in file)
        {
            if (line != "")
            {
                PersistenceInfo persistenceInfo = JsonConvert.DeserializeObject<PersistenceInfo>(line);
                if (persistenceInfo.id != -1)
                {

                    gmL[i].GetComponent<PersistenceGameobject>().id = persistenceInfo.id;
                    gmL[i].GetComponent<PersistenceGameobject>().name = persistenceInfo.name;

                    gmL[i].GetComponent<PersistenceGameobject>().loadPersistenceTransform();
                    gmL[i].GetComponent<PersistenceGameobject>().loadPersistenceComponents();
                   
                    i++;
                }
            }

           
        }


        /*
        i = 0;
        foreach (var line in file)
        {
            if (line != "")
            {
                PersistenceInfo persistenceInfo = JsonConvert.DeserializeObject<PersistenceInfo>(line);
                if (persistenceInfo.id != -1)
                {
                    gmL[i].GetComponent<PersistenceGameobject>().loadPersistenceComponents();
                    i++;
                }
            }
        }

    */

    }

    public void saveScenary()
    {
        try
        {
            //_fileBrowser.deleteFileInActualPath(_nameOfSavegameInput.text + ".txt");

            //Create or append in a file add the header with the information of this activity

            SavegameInfo savegameInfo = new SavegameInfo();

            savegameInfo.createdTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            savegameInfo.name = (_saveGameName != "") ? _saveGameName : savegameInfo.createdTime;
            savegameInfo.description = _saveGameDescription;
            //savegameInfo.savegameTime = FindObjectOfType<DayNight>().currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            savegameInfo.savegameTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //_fileBrowser.editFileInActualPath(
            //    JsonUtility.ToJson(savegameInfo)
            //    , _nameOfSavegameInput.text + ".txt"

            //);
            
            //save the gameobjects loaded in gameobjectlist
            saveGameObjectsInJSON(_saveGameName + ".txt");
           // _fileBrowser.createFolder(_nameOfSavegameInput.text, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VirtualADLSimulator");
           // _fileBrowser.refreshGridContentOfActualPath();
            updateFileBrowserItemListeners();

    
            //_fileBrowser.confirmMessage.showMessage("Scenary " + _nameOfSavegameInput.text + " saved!");

            
        }    
        catch (Exception e)
        {
           // _fileBrowser.confirmMessage.showMessage("Error saving the scenary " + _nameOfSavegameInput + "! " + e.Message);
            //return false;
        }

        //return true;
    }


    public bool removeScenary()
    {
        try
        {
           // _fileBrowser.deleteFileInActualPath(_nameOfSavegameInput.text + ".txt");
            _saveGameName = "";
            _saveGameDescription = "";

           // _fileBrowser.refreshGridContentOfActualPath();
            updateFileBrowserItemListeners();
        }
        catch (Exception e)
        {
           // _fileBrowser.confirmMessage.showMessage("Error saving: " + e.Message);
            return false; 
        }
        return true;
    }

    public bool loadScenary()
    {
        try
        {
           // FindObjectOfType<SceneAndURLLoader>().sceneLoad("MainScene", "savegames/" + _nameOfSavegameInput.text + ".txt");

        }
        catch(Exception e)
        {
            Debug.Log(e);
            return false;
        }
        return true;
    }


    //public bool removeExperimentsAssociated()
    //{
    //    try
    //    {
    //        return _fileBrowser.removeFolder(_nameOfSavegameInput.text, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/VirtualADLSimulator", true);

    //    }
    //    catch (Exception e)
    //    {
    //        throw e;
    //    }
    //}
}
