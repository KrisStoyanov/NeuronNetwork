using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private Stack<Menu> menus = new Stack<Menu>();
    //private MainScreen mainScreen = null;

    private MenuManager()
    {

    }

    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {

    }

    private void OnDestroy()
    {

    }
    public static Menu GetMenu(string menuName, params object[] args)
    {
        Menu newMenu = ScriptableObject.CreateInstance(menuName) as Menu;
       // if(menuName == nameof(HUD))
        //{
            
        //}
        newMenu.Init(args);
        return newMenu;
    }

    public void Push(string menuType, params object[] args)
    {
        menus.Push(GetMenu(menuType, args));
        menus.Peek().Show();
    }

    public void Pop()
    {
        menus.Peek().Hide();
        menus.Pop();
    }

    public void PopAll()
    {
        while (menus.Count != 0)
        {
            Pop();
        }
    }
}
