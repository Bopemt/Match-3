using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    private Board board;
    private FindMatches finder;
    [SerializeField] private float hintDelay;
    public float hintDelaySeconds;
    [SerializeField] private GameObject hintParticle;
    public List<GameObject> possibleMoves = new List<GameObject>();

    void Start()
    {
        board = FindObjectOfType<Board>();
        finder = FindObjectOfType<FindMatches>();
        RestartTimer();
    }

    void Update()
    {
        if (board.currentState == GameState.move)
        {
            if (hintDelaySeconds <= 0)
            {
                RestartTimer();
                FindPossibleMoves();
                if (possibleMoves.Count > 0) 
                {
                    Vector3 position = possibleMoves[Random.Range(0, possibleMoves.Count)].GetComponent<Dot>().logicPosition;
                    Instantiate(hintParticle, position, Quaternion.identity);
                }
            }
            else
            {
                hintDelaySeconds-=Time.deltaTime;
            }
        }
    }

    public void RestartTimer()
    {
        hintDelaySeconds = hintDelay;
    }

    public void FindPossibleMoves()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.tiles[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (finder.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.tiles[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (finder.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.tiles[i, j]);
                        }
                    }
                }
            }
        }
    }

    public void ClearPossibleMovesList()
    {
        possibleMoves.Clear();
    }
}
