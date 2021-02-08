using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneButton : MonoBehaviour
{

    public bool LoadGame = false;
    public bool LoadPrologue = false;
    public bool LoadStartMenu = false;

    SceneLoader Loader;

    void Awake()
    {
        Loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
    }

    // Update is called once per frame
    public void Load()
    {


        if (LoadGame)
        {
            Loader.LoadGameScene();
        }
        else if (LoadPrologue)
        {
            Loader.LoadDialog();
        }
        else if (LoadStartMenu)

        {
            Loader.LoadStartMenu();
        }
    }
}
