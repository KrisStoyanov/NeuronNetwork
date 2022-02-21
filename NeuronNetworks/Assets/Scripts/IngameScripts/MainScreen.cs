using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScreen : MonoBehaviour
{
    private bool isBackProp = false;
    private int layers = 0;
    private int children = 0;
    private int neurons = 0;
    private Transform menu = null;
    public static event Action<bool,int,int,int> StartGame;
    
    void Start()
    {
        menu = this.gameObject.transform;
        menu.transform.Find("BackPropButton").GetComponent<Button>().onClick.AddListener(delegate { BackPropClicked(); });
        menu.transform.Find("MutationButton").GetComponent<Button>().onClick.AddListener(delegate { MutationDrivenClicked(); });
        menu.transform.Find("PlayButton").GetComponent<Button>().onClick.AddListener(delegate { PlayButtonClicked(); });
        menu.transform.Find("PlayButton").gameObject.SetActive(false);
    }
    private void BackPropClicked()
    {
        isBackProp = true;
        menu.transform.Find("PlayButton").gameObject.SetActive(true);
    }
    private void MutationDrivenClicked()
    {
        isBackProp = false;
        menu.transform.Find("PlayButton").gameObject.SetActive(true);
    }
    public void PlayButtonClicked()
    {
        layers = int.Parse(menu.transform.Find("InputHiddenLayers").transform.GetComponent<TMP_InputField>().text);
        children = int.Parse(menu.transform.Find("InputChildrenInGeneration").transform.GetComponent<TMP_InputField>().text);
        neurons = int.Parse(menu.transform.Find("InputNumberOfNeuronsInLayer").transform.GetComponent<TMP_InputField>().text);
        menu.transform.parent.transform.Find("GameManagers").gameObject.SetActive(true);
        menu.transform.parent.transform.Find("HUD").gameObject.SetActive(true);
        
        GameObject[] objects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var item in objects)
        {
            if(item.name=="MovingPrefabs")
            {
                item.GetComponent<Movement>().enabled = true;
            }
        }
        StartCoroutine(Coroutine());
        
    }

    public IEnumerator Coroutine()
    {
        yield return new WaitForSeconds(0.1f);
        StartGame?.Invoke(isBackProp, children, layers, neurons);
        GameObject.Destroy(this.gameObject);
    }

}
