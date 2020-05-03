using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    public static bool pauseFlag = false;
    public static GameObject selectionmanager;
    public static GameObject optionsmanager;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (selectionmanager.activeInHierarchy == false) 
            {
                if (optionsmanager.activeInHierarchy == false)
                {
                    if (pauseFlag == false)
                    {
                        Time.timeScale = 0.0f;
                        transform.GetChild(0).gameObject.SetActive(true);
                        transform.GetChild(1).gameObject.SetActive(true);

                        pauseFlag = true;
                    }
                    else
                    {
                        Time.timeScale = 1.0f;
                        transform.GetChild(0).gameObject.SetActive(false);
                        transform.GetChild(1).gameObject.SetActive(false);

                        pauseFlag = false;
                    }
                }
            }
        }
    }
}
