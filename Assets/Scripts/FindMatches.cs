using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.tiles[i, j];
                GameObject leftDot = null;
                GameObject rightDot = null;
                GameObject downDot = null;
                GameObject upDot = null;
                if (i > 0 && i < board.width - 1)
                {
                    leftDot = board.tiles[i - 1, j];
                    rightDot = board.tiles[i + 1, j];
                }
                if (j > 0 && j < board.height - 1)
                {
                    downDot = board.tiles[i, j - 1];
                    upDot = board.tiles[i, j + 1];
                }
                if (currentDot != null)
                {
                    if (leftDot != null && rightDot != null)
                    {
                        if (currentDot.CompareTag(leftDot.tag) && currentDot.CompareTag(rightDot.tag))
                        {
                            SetMatchedAndTriggered(leftDot);
                            SetMatchedAndTriggered(rightDot);
                            SetMatchedAndTriggered(currentDot);
                        }
                    }
                    if (downDot != null && upDot != null)
                    {
                        if (currentDot.CompareTag(downDot.tag) && currentDot.CompareTag(upDot.tag))
                        {
                            SetMatchedAndTriggered(upDot);
                            SetMatchedAndTriggered(downDot);
                            SetMatchedAndTriggered(currentDot);
                        }
                    }
                }
            }
        }
    }

    private void SetMatchedAndTriggered(GameObject dot)
    {
        dot.GetComponent<Dot>().isMatched = true;
        dot.GetComponent<Dot>().anim.SetTrigger("Destroy");
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        GameObject tmp = board.tiles[column + (int)direction.x, row + (int)direction.y] as GameObject;
        board.tiles[column + (int)direction.x, row + (int)direction.y] = board.tiles[column, row];
        board.tiles[column, row] = tmp;
    }

    private bool CheckMatches()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.tiles[i, j];
                GameObject leftDot = null;
                GameObject rightDot = null;
                GameObject downDot = null;
                GameObject upDot = null;
                if (i > 0 && i < board.width - 1)
                {
                    leftDot = board.tiles[i - 1, j];
                    rightDot = board.tiles[i + 1, j];
                }
                if (j > 0 && j < board.height - 1)
                {
                    downDot = board.tiles[i, j - 1];
                    upDot = board.tiles[i, j + 1];
                }
                if (currentDot != null)
                {
                    if (leftDot != null && rightDot != null)
                    {
                        if (currentDot.CompareTag(leftDot.tag) && currentDot.CompareTag(rightDot.tag))
                        {
                            return true;
                        }
                    }
                    if (downDot != null && upDot != null)
                    {
                        if (currentDot.CompareTag(downDot.tag) && currentDot.CompareTag(upDot.tag))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    public bool IsGameOver()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.tiles[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
}
