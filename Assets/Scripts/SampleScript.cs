using UnityEngine;

public class SampleScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    // Start is called before the first frame update
    void Start()
    {
        inputReader.PauseEvent += PauseGame;
    }

    // Update is called once per frame
    void PauseGame()
    {
        Debug.Log("Paused game? :)");
    }
}

