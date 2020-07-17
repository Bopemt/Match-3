using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum GameState
{
    wait,
    move,
    starting,
    pause
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.starting;
    [SerializeField] private GameObject[] dots;
    public int width, height, offSet;
    public GameObject[,] tiles;
    public FindMatches finder;
    private HintManager hint;
    private Menu menu;

    void OnEnable()
    {
        menu = FindObjectOfType<Menu>();
        hint = FindObjectOfType<HintManager>();
        finder = FindObjectOfType<FindMatches>();
        tiles = new GameObject[width, height];
        menu.restartBtn.gameObject.SetActive(false);
        menu.gameOverTMP.gameObject.SetActive(false);
        CreateBoard();
    }

    private void Update()
    {
        if (currentState == GameState.move)
            if (finder.IsGameOver())
                StartCoroutine(DisableCo());
    }

    private IEnumerator DisableCo()
    {
        yield return new WaitForSeconds(.8f);
        this.enabled = false;
    }

    private void OnDisable()
    {
        currentState = GameState.starting;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null)
                {
                        Destroy(tiles[i, j]);
                        tiles[i, j] = null;
                }
            }
        }
        hint.RestartTimer();
        hint.ClearPossibleMovesList();
        if(menu.restartBtn)
            menu.restartBtn.gameObject.SetActive(true);
        if (menu.gameOverTMP)
            menu.gameOverTMP.gameObject.SetActive(true);
    }

    private void CreateBoard()
    {
        GameObject[] previousLeft1 = new GameObject[height];
        GameObject previousBelow1 = null;
        GameObject[] previousLeft2 = new GameObject[height];
        GameObject previousBelow2 = null;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GameObject> possibleDots = new List<GameObject>();
                possibleDots.AddRange(dots);

                if(previousBelow1 == previousBelow2)
                {
                    possibleDots.Remove(previousBelow1);
                }
                if(previousLeft1[j] == previousLeft2[j])
                {
                    possibleDots.Remove(previousLeft1[j]);
                }

                GameObject dotToUse = possibleDots[Random.Range(0, possibleDots.Count)];
                GameObject dot = Instantiate(dotToUse, new Vector2(i, j + offSet), Quaternion.identity);
                dot.GetComponent<Dot>().logicPosition.x = i;
                dot.GetComponent<Dot>().logicPosition.y = j;
                dot.transform.parent = this.transform;
                dot.name = "(" + i + ", " + j + ")";
                tiles[i, j] = dot;

                previousLeft2[j] = previousLeft1[j];
                previousBelow2 = previousBelow1;
                previousLeft1[j] = dotToUse;
                previousBelow1 = dotToUse;
            }
        }

        StartCoroutine(StartTimeCo());
    }

    private IEnumerator StartTimeCo()
    {
        yield return new WaitForSeconds(.8f);
        currentState = GameState.move;
    }

    public void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (tiles[i, j].GetComponent<Dot>().isMatched)
                    {
                        Destroy(tiles[i, j]);
                        tiles[i, j] = null;
                    }
                }
            }
        }
        hint.RestartTimer();
        hint.ClearPossibleMovesList();
        StartCoroutine(DecreaseRowCo());
    }

    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    tiles[i, j].GetComponent<Dot>().logicPosition.y -= nullCount;
                    tiles[i, j].name = "(" + i + ", " + tiles[i, j].GetComponent<Dot>().logicPosition.y + ")";
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] == null)
                {
                    GameObject piece = Instantiate(dots[Random.Range(0, dots.Length)], new Vector2(i, j + offSet), Quaternion.identity);
                    piece.transform.parent = this.transform;
                    piece.name = "(" + i + ", " + j + ")";
                    tiles[i, j] = piece;
                    piece.GetComponent<Dot>().logicPosition.x = i;
                    piece.GetComponent<Dot>().logicPosition.y = j;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tiles[i, j] != null)
                {
                    if (tiles[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.4f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.4f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.4f);
        currentState = GameState.move;
    }
}
