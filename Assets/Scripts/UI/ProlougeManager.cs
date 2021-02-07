using System.Collections;
using System.Collections.Generic;
using RedBlueGames.Tools.TextTyper;
using TMPro;
using UnityEngine;

public class ProlougeManager : MonoBehaviour
{
    public InputReader InputReader;

    public TMPro.TMP_Text Talker;
    public TextTyper Dialog;


    public SceneLoader Loader;

    private Queue<(string Talker, string Content)> dialogueLines = new Queue<(string Talker, string Content)>();

    // Start is called before the first frame update
    void Start()
    {
        Loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();
        InputReader.EnableMenuInput();
        InputReader.SkipEvent += () =>
        {
            if (dialogueLines.Count == 0)
            {
                InputReader.DisableMenuInput();
                Loader.LoadGameScene();
            }
            else
            {
                Skip();
            }
        };

        ShowScript();
    }

    public void Skip()
    {
        if (Dialog.IsSkippable() && Dialog.IsTyping)
        {
            Dialog.Skip();
        }
        else
        {
            ShowScript();
        }
    }

    private void ShowScript()
    {
        if (dialogueLines.Count <= 0)
        {
            return;
        }
        var (talker, content) = dialogueLines.Dequeue();
        Talker.text = talker;
        Dialog.TypeText(content);
    }
}
