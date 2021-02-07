using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public Animator Transition;
    public float TransitionTime;

    public readonly static int MENU_SCENE = 1;
    public readonly static int GAMEPLAY_SCENE = 2;
    public readonly static int DIALOG_SCENE = 3;

    public void Awake()
    {
        Transition.gameObject.SetActive(false);
        SceneManager.LoadScene(MENU_SCENE, LoadSceneMode.Additive);
    }

    public void LoadDialog()
    {
        StartCoroutine(LoadScene(DIALOG_SCENE, MENU_SCENE));
    }

    public void LoadGameScene()
    {
        StartCoroutine(LoadScene(GAMEPLAY_SCENE, MENU_SCENE));
    }

    public void LoadStartMenu()
    {
        StartCoroutine(LoadScene(MENU_SCENE, GAMEPLAY_SCENE));
    }

    IEnumerator LoadScene(int sceneIndex, int unloadScene, bool down = false)
    {
        Transition.gameObject.SetActive(true);
        Transition.SetTrigger("Hide");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.UnloadSceneAsync(unloadScene);
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Additive);
        yield return new WaitForSeconds(0.33f);
        Transition.SetTrigger("Reveal");
        yield return new WaitForSeconds(TransitionTime);
        Transition.gameObject.SetActive(false);
    }
}
