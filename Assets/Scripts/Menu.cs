using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    private Board board;
    [SerializeField] private Button startBtn, pauseBtn, musicBtn;
    [SerializeField] private TextMeshProUGUI nameTMP;
    [SerializeField] private GameObject blurPanel;
    private Vector3 temp;
    private bool pause = false, music = true, pressed = false;
    private GameState prevState;
    public Button restartBtn;
    public TextMeshProUGUI gameOverTMP;

    void Start()
    {
        board = FindObjectOfType<Board>();
        temp = startBtn.transform.position;
        temp.y -= board.offSet * 20;
    }

    private void Update()
    {
        if (pressed)
        {
            startBtn.transform.position = Vector2.Lerp(startBtn.transform.position, temp, .1f);
        }
    }

    public void StartBTN()
    {
        board.enabled = true;
        pressed = true;
        startBtn.interactable = false;
        nameTMP.enabled = false;
        pauseBtn.gameObject.SetActive(true);
    }

    public void MusicBTN()
    {
        if (music)
        {
            musicBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough;
            AudioListener.pause = !AudioListener.pause;
            music = !music;
        }
        else
        {
            musicBtn.GetComponentInChildren<TextMeshProUGUI>().fontStyle = FontStyles.Normal;
            AudioListener.pause = !AudioListener.pause;
            music = !music;
        }
    }

    public void PauseBTN()
    {
        if (pause)
        {
            blurPanel.SetActive(false);
            pauseBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
            board.currentState = prevState;
            Time.timeScale = 1;
            pause = !pause;
        }
        else
        {
            blurPanel.SetActive(true);
            pauseBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
            prevState = board.currentState;
            board.currentState = GameState.pause;
            Time.timeScale = 0;
            pause = !pause;
        }
    }

    public void RestartBTN()
    {
        board.enabled = true;
    }
}
