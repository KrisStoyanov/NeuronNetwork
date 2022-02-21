using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenMenu : MonoBehaviour
{
    Button startButton = null;
    void Start()
    {
        MenusGameManager.RemoveLoadingScreen += MenusGameManager_RemoveLoadingScreen;
        startButton = this.gameObject.transform.Find("Button").GetComponent<Button>();
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(delegate { StartGame(); });
        SetupPlayerInfo();
    }
    private void SetupPlayerInfo()
    {
        //LoadPlayerInfo.CreateInstance();
    }

    private void MenusGameManager_RemoveLoadingScreen()
    {
        MenusGameManager.RemoveLoadingScreen -= MenusGameManager_RemoveLoadingScreen;
    }
    private void StartGame()
    {
       // MenuManager.Instance.PopAll();
        //MenuManager.Instance.Push(nameof(MainMenu));
        Destroy(this.gameObject);
    }

    void Update()
    {
        
    }
}
