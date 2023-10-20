﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PersistenceGameobject : MonoBehaviour
{
    public static int autoCounter = 0;
    public int id;
    public PersistenceInfo persistenceInfo;
    public bool loaded = false;
    
    private void Awake()
    {
        id = autoCounter;
        autoCounter++;
    }

    // Use this for initialization
    void Start()
    {
        if (!loaded)
            updatePersistenceInfo();
    }

    public static void ResetIdCounter()
    {
        autoCounter = 0;
    }

    public void updatePersistenceInfo()
    {
        persistenceInfo.id = id;
        persistenceInfo.name = gameObject.name;

        loadPersistenceComponentsInfo();
       
    }

    public void loadPersistenceTransform()
    {
        foreach (JObject jObj in persistenceInfo.serializableComponents)
            if (jObj.GetValue("Type").ToString() == typeof(PersistenceTransform).ToString())
                jObj.ToObject<PersistenceTransform>().addComponentInGameobject(gameObject);
    }
  
    
    public void loadPersistenceComponents()
    {
        foreach (JObject jObj in persistenceInfo.serializableComponents)
        {
            if (jObj.GetValue("Type").ToString() == typeof(PersistenceRigidbody).ToString())
                jObj.ToObject<PersistenceRigidbody>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceMeshFilter).ToString())
                jObj.ToObject<PersistenceMeshFilter>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceMeshRenderer).ToString())
                jObj.ToObject<PersistenceMeshRenderer>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceCollider).ToString())
                jObj.ToObject<PersistenceCollider>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceTextMesh).ToString())
                jObj.ToObject<PersistenceTextMesh>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceAudioSource).ToString())
                jObj.ToObject<PersistenceAudioSource>().addComponentInGameobject(gameObject);

            if (jObj.GetValue("Type").ToString() == typeof(PersistenceLight).ToString())
                jObj.ToObject<PersistenceLight>().addComponentInGameobject(gameObject);
        }
    }

    public static T findObjectInResources<T>(string name, string folder = "")
    {
        if (typeof(T) == typeof(Mesh))
            foreach (var gm in Resources.LoadAll<Mesh>(folder))
                if (gm.name == name)
                    return (T)Convert.ChangeType(gm, typeof(T));

        if (typeof(T) == typeof(Material))
            foreach (var gm in Resources.LoadAll<Material>(folder))
                if (gm.name == name)
                    return (T)Convert.ChangeType(gm, typeof(T));

        if (typeof(T) == typeof(AudioClip))
            foreach (var gm in Resources.LoadAll<AudioClip>(folder))
                if (gm.name == name)
                    return (T)Convert.ChangeType(gm, typeof(T));

        return default(T);
    }

    public void loadPersistenceComponentsInfo()
    {
        persistenceInfo.serializableComponents.Clear();

        foreach(var comp in GetComponents<Component>())
        {
            
            switch (comp.GetType().ToString())
            {
                case "UnityEngine.Transform":
                    PersistenceTransform persistenceTransform = new PersistenceTransform();
                    persistenceInfo.serializableComponents.Add(persistenceTransform.loadComponentInfo(gameObject));
                    break;

                case "UnityEngine.Rigidbody":
                    PersistenceRigidbody persistenceRigidbody = new PersistenceRigidbody();
                    persistenceRigidbody.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceRigidbody);
                    break;

                case "UnityEngine.MeshFilter":
                    PersistenceMeshFilter persistenceMeshFilter = new PersistenceMeshFilter();
                    persistenceMeshFilter.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceMeshFilter);
                    break;

                case "UnityEngine.MeshRenderer":
                    PersistenceMeshRenderer persistenceMeshRenderer = new PersistenceMeshRenderer();
                    persistenceMeshRenderer.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceMeshRenderer);

                    break;

                case "UnityEngine.MeshCollider":
                    PersistenceCollider persistenceCollider = new PersistenceCollider();
                    persistenceCollider.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceCollider);
                    break;

                case "UnityEngine.BoxCollider":
                    persistenceCollider = new PersistenceCollider();
                    persistenceCollider.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceCollider);
                    break;

                case "UnityEngine.TextMesh":
                    PersistenceTextMesh persistenceTextMesh = new PersistenceTextMesh();
                    persistenceTextMesh.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceTextMesh);
                    break;

                case "UnityEngine.AudioSource":
                    PersistenceAudioSource persistenceAudioSource = new PersistenceAudioSource();
                    persistenceAudioSource.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceAudioSource);
                    break;

                case "UnityEngine.Light":
                    PersistenceLight persistenceLight = new PersistenceLight();
                    persistenceLight.loadComponentInfo(gameObject);
                    persistenceInfo.serializableComponents.Add(persistenceLight);
                    break;
            } 
        }
    }
}
