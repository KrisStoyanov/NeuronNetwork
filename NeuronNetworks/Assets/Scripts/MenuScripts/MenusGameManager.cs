using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MenusGameManager : MonoBehaviour
{
    public static event Action RemoveLoadingScreen;
    void Start()
    {
        if(StaticClass.CrossSceneInformation=="No")
        {
            RemoveLoadingScreen?.Invoke();
            StaticClass.CrossSceneInformation = "Yes";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
