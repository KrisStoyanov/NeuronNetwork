using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Menu : ScriptableObject
{
    public static List<Menu> menus;

    protected GameObject menuUI = null;

    public abstract string Name { get; }
    private string beginOfPath = "Prefabs/Menus/";

    public Menu()
    {
        //Used for menu factory
    }

    public Menu(params object[] args)
    {

    }

    public abstract void Init(params object[] args);

    private void UpdateEvent_onUpdatePerSecond()
    {
        UpdatePerSecond();
    }

    private void UpdateEvent_EventUpdate(float deltaTime)
    {
        UpdatePerFrame(deltaTime);
    }

    protected virtual void LoadMenu(string resourcesPath)
    {
        menuUI = (GameObject)Instantiate(Resources.Load<GameObject>(beginOfPath+resourcesPath));
        menuUI.name = resourcesPath;
        var canvas = menuUI.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        //if (menuUI.name == nameof(HUD))
        //{
            
        //    canvas.planeDistance = 3;
        //}
        //else
        //{
        //    canvas.planeDistance = 2;
        //}
    }

    public virtual void Show()
    {
        UpdateEvent.onUpdateScaledTime += UpdateEvent_EventUpdate;
        UpdateEvent.onUpdatePerSecond += UpdateEvent_onUpdatePerSecond;
    }

    public virtual void Hide()
    {
        UpdateEvent.onUpdateScaledTime -= UpdateEvent_EventUpdate;
        UpdateEvent.onUpdatePerSecond -= UpdateEvent_onUpdatePerSecond;
        Destroy(menuUI);
    }

    protected virtual void UpdatePerFrame(float deltaTime)
    {

    }

    protected virtual void UpdatePerSecond()
    {

    }
}
