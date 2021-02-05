using UnityEngine;

public class SampleScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    // Start is called before the first frame update
    void Start()
    {
        inputReader.PauseEvent += PauseGame;
        inputReader.MoveEvent += (float axis) => Debug.Log("Moving! Direction: " + axis);
        inputReader.JumpEvent += () => Debug.Log("Jump!");
        inputReader.InteractEvent += () => Debug.Log("Interacting!");
        inputReader.HoldEvent += () => Debug.Log("Hold!");
    }

    // Update is called once per frame
    void PauseGame()
    {
        Debug.Log("Paused game? :)");
    }
}

