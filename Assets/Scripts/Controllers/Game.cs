using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    [Header("Game Entities")]
    public Transform StartPoint;
    public Wire StartWire;
    public Generator Generator;
    public GameObject Player;

    [Header("UI")]
    public GameObject ReloadText;
    public GameObject EndScore;
    public TMPro.TMP_Text EndScoreText;
    public GameObject BoostIndicator;

    private SceneLoader Loader;

    private WireHolder Holder;

    [SerializeField]
    private InputReader InputReader;
    private MovementController PlayerMc;

    private bool reachedMax = false;
    private bool restarting = false;
    private bool wonGame = false;

    void Start()
    {
        Loader = GameObject.FindWithTag("SceneLoader").GetComponent<SceneLoader>();

        InputReader.EnableGameplayInput();
        InputReader.DisableMenuInput();
        EndScore.SetActive(false);

        InputReader.ReloadEvent += () =>
        {
            InputReader.Clear();
            Restart();
            restarting = true;
        };

        Generator.OnInserted += () =>
        {
            wonGame = true;
            EndGame();
        };

        Holder = Player.GetComponent<WireHolder>();
        Holder.GetComponent<WireHolder>().WireReachedMaxLength += () =>
        {
            JSAM.AudioManager.PlaySound(JSAM.Sounds.Death);
            ReloadText.SetActive(true);
            reachedMax = true;
        };

        PlayerMc = Player.GetComponent<MovementController>();

    }


    public void Update()
    {
        if ((reachedMax && !Holder.Wire.AtMaxLength) || wonGame)
        {
            ReloadText.SetActive(false);
        }

        BoostIndicator.SetActive(PlayerMc.HasBoost);

    }

    public void Restart()
    {
        if (restarting) return;
        Loader.RestartGameScene();
        JSAM.AudioManager.PlaySound(JSAM.Sounds.Restart);
    }

    public void EndGame()
    {
        var length = 0f;

        foreach (var wire in FindObjectsOfType<Wire>())
        {
            length += wire.TotalLength;
        }

        EndScoreText.text = $"{length}m";
        EndScore.SetActive(true);
    }
}
