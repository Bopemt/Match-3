using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Image greenImage, redImage;
    [SerializeField] private float duration;
    private float durationSeconds;
    [SerializeField] private Board board;

    void OnEnable()
    {
        board = FindObjectOfType<Board>();
        greenImage.enabled = true;
        redImage.enabled = true;
        greenImage.fillAmount = 1;
        durationSeconds = duration;
    }

    private void OnDisable()
    {
        greenImage.enabled = false;
        redImage.enabled = false;
    }

    void Update()
    {
        if (durationSeconds <= 0)
        {
            board.enabled = false;
            this.enabled = false;
        }
        else if (board.currentState == GameState.move)
        {
            durationSeconds -= Time.deltaTime;
            greenImage.fillAmount = durationSeconds / duration;
        }
    }
}
