using System.Collections;
using System.Collections.Generic;
using RedBlueGames.Tools.TextTyper;
using UnityEngine;

public class ProlougeManager : MonoBehaviour
{
    public InputReader InputReader;
    public TextTyper Dialog;
    public SceneLoader Loader;

    private Queue<string> dialogueLines = new Queue<string>();

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

        dialogueLines.Enqueue("Hello! My name is... <delay=0.5>NPC</delay>. Got it, <i>bub</i>?");
        dialogueLines.Enqueue("You can <b>use</b> <i>uGUI</i> <size=40>text</size> <size=20>tag</size> and <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
        dialogueLines.Enqueue("bold <b>text</b> test <b>bold</b> text <b>test</b>");
        dialogueLines.Enqueue("Sprites!<sprite index=0><sprite index=1><sprite index=2><sprite index=3>Isn't that neat?");
        dialogueLines.Enqueue("You can <size=40>size 40</size> and <size=20>size 20</size>");
        dialogueLines.Enqueue("You can <color=#ff0000ff>color</color> tag <color=#00ff00ff>like this</color>.");
        dialogueLines.Enqueue("Sample Shake Animations: <anim=lightrot>Light Rotation</anim>, <anim=lightpos>Light Position</anim>, <anim=fullshake>Full Shake</anim>\nSample Curve Animations: <animation=slowsine>Slow Sine</animation>, <animation=bounce>Bounce Bounce</animation>, <animation=crazyflip>Crazy Flip</animation>");
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

        Dialog.TypeText(dialogueLines.Dequeue());
    }
}
