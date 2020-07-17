using System.Collections;
using UnityEngine;

public class Dot : MonoBehaviour
{
    private Vector2 firstTouchPosition, finalTouchPosition;
    private float swipeAngle = 0;
    [SerializeField] private float swipeResist = 1f;
    private Board board;
    private Vector3 previousPosition;
    public Vector3 logicPosition;
    private GameObject otherDot;
    public bool isMatched = false;
    private Vector3 target;
    private SpriteRenderer mySprite;
    public Animator anim;
    private FindMatches finder;

    void Start()
    {
        anim = GetComponent<Animator>();
        board = FindObjectOfType<Board>();
        target = transform.position;
        mySprite = GetComponent<SpriteRenderer>();
        finder = FindObjectOfType<FindMatches>();
    }

    void Update()
    {
        if (isMatched)
        {
            var tmpColor = mySprite.color;
            tmpColor.a = 0.8f;
            mySprite.color = tmpColor;
        }
        target = logicPosition;
        if (Mathf.Abs((target - transform.position).magnitude) > .05f)
        {
            transform.position = Vector2.Lerp(transform.position, target, .1f);
            if (board.tiles[(int)logicPosition.x, (int)logicPosition.y] != this.gameObject)
            {
                board.tiles[(int)logicPosition.x, (int)logicPosition.y] = this.gameObject;
            }
            if (board.currentState != GameState.starting)
                finder.FindAllMatches();
        }
        else
        {
            transform.position = target;
            board.tiles[(int)logicPosition.x, (int)logicPosition.y] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        if(board.currentState == GameState.move)
            firstTouchPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPosition = Input.mousePosition;
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MoveDots();
        }
    }

    void MoveDots()
    {
        previousPosition = transform.position;
        if (swipeAngle > -45 && swipeAngle <= 45 && transform.position.x < board.width - 1) //Right
        {
            otherDot = board.tiles[(int)logicPosition.x + 1, (int)logicPosition.y];
            if (otherDot)
            {
                Swap(logicPosition, new Vector3((int)logicPosition.x + 1, (int)logicPosition.y));
            }
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && transform.position.y < board.height - 1) //Up
        {
            otherDot = board.tiles[(int)logicPosition.x, (int)logicPosition.y + 1];
            if (otherDot)
            {
                Swap(logicPosition, new Vector3((int)logicPosition.x, (int)logicPosition.y + 1));
            }
        }
        else if (swipeAngle > 135 || swipeAngle <= -135 && transform.position.x > 0) //Left
        {
            otherDot = board.tiles[(int)logicPosition.x - 1, (int)logicPosition.y];
            if (otherDot)
            {
                Swap(logicPosition, new Vector3((int)logicPosition.x - 1, (int)logicPosition.y));
            }
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && transform.position.y > 0) //Down
        {
            otherDot = board.tiles[(int)logicPosition.x, (int)logicPosition.y - 1];
            if (otherDot)
            {
                Swap(logicPosition, new Vector3((int)logicPosition.x, (int)logicPosition.y - 1));
            }
        }
        StartCoroutine(CheckMoveCo());
    }

    void Swap(Vector3 position, Vector3 newPosition)
    {
        board.tiles[(int)position.x, (int)position.y].GetComponent<Dot>().logicPosition = new Vector2(newPosition.x, newPosition.y);
        board.tiles[(int)newPosition.x, (int)newPosition.y].GetComponent<Dot>().logicPosition = new Vector2(position.x, position.y);
        board.currentState = GameState.wait;
    }

    private IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.25f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().logicPosition = transform.position;
                logicPosition = previousPosition;
                yield return new WaitForSeconds(.3f);
                board.currentState = GameState.move;
            }
            else
            {
                board.Invoke("DestroyMatches", .2f);
            }
            otherDot = null;
        }
    }
}