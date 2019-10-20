using UnityEngine;
using System.Collections;
using System;

public class ShooterKitMainMenu : MonoBehaviour {

    private MenuBase mMenuBase;

    public GameObject SubMenu;
    public GameObject MainMenu;

    private bool bInit = false;

    void OnEnable()
    {
        mMenuBase = gameObject.GetComponent<MenuBase>();

        if (mMenuBase == null)
        {
            throw new Exception("Menu base missing");
        }

        bInit = true;
    }


    void Update()
    {
        if (bInit == false) return;


    }
}
