using System.Collections;
using System.Collections.Generic;
using RedBlueGames.Tools.TextTyper;
using TMPro;
using UnityEngine;

public class ProlougeManager : MonoBehaviour
{
    public InputReader InputReader;
    public GameObject VillageScene;
    public TMPro.TMP_Text Talker;
    public TextTyper Dialog;

    public GameObject Buttons;

    public SceneLoader Loader;

    private Queue<(string Talker, string Content)> dialogueLines = new Queue<(string Talker, string Content)>();

    // Start is called before the first frame update
    void Start()
    {
        Loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();

        AddDialog("Old Outlettian", "Fellow Outlettians, we’re almost out of all our power, the outlet king pulled the plug on us! We need someone with enough energy and will to help us reconnect!");
        AddDialog("You", "I can do it!");
        AddDialog("Old Outlettian", "You sure? It is a long journey over to the holy generator.");
        AddDialog("You", "I am sure. I will connect us back again!");
        AddDialog("Old Outlettian", "Great! Although we do not have much cable to spare, you will have to do with what we have got. But keep a lookout for power strips to ease the journey to our great generator.");

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

    void OnEnable()
    {
        VillageScene.SetActive(true);
    }

    void OnDisable()
    {
        VillageScene.SetActive(false);
    }

    public void AddDialog(string talker, string content)
    {
        dialogueLines.Enqueue((Talker: talker, Content: content));
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
