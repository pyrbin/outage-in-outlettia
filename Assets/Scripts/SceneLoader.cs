using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public Animator Transition;
    public float TransitionTime;
    public InputReader InputReader;

    public readonly static int MENU_SCENE = 1;
    public readonly static int GAMEPLAY_SCENE = 2;
    public readonly static int DIALOG_SCENE = 3;

    public bool DontLoadMenuOnStart = false;

    public void Awake()
    {
        Transition.gameObject.SetActive(false);
        if (!DontLoadMenuOnStart)
            SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Additive);
        else
        {
            Transition.SetTrigger("Reveal");
        }
    }

    public void LoadDialog()
    {
        StartCoroutine(LoadScene(DIALOG_SCENE, MENU_SCENE));
    }

    public void RestartGameScene()
    {
        StartCoroutine(LoadScene(GAMEPLAY_SCENE, GAMEPLAY_SCENE, false));
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadScene(GAMEPLAY_SCENE, MENU_SCENE));
    }

    public void LoadStartMenu()
    {
        StartCoroutine(LoadScene(MENU_SCENE, GAMEPLAY_SCENE));
    }

    IEnumerator LoadScene(int sceneIndex, int unloadScene, bool music = true)
    {
        InputReader.Clear();
        Transition.gameObject.SetActive(true);
        Transition.SetTrigger("Hide");
        yield return new WaitForSeconds(TransitionTime);
        var operation = SceneManager.UnloadSceneAsync(unloadScene);
        while (!operation.isDone)
        {
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.33f);
        Transition.SetTrigger("Reveal");


        if (music)
        {
            if (sceneIndex == GAMEPLAY_SCENE)
                JSAM.AudioManager.PlayMusic(JSAM.Music.Music_game);
            if (sceneIndex == MENU_SCENE)
                JSAM.AudioManager.PlayMusic(JSAM.Music.Music_Title);
        }



        yield return new WaitForSeconds(TransitionTime);
        Transition.gameObject.SetActive(false);
    }
}
