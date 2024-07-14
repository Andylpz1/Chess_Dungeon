using UnityEngine;

public class Player : MonoBehaviour
{
    public Vector2Int position; // 棋子在棋盘上的位置
    public int boardSize = 5;   // 棋盘大小
    public GameObject moveHighlightPrefab; // 用于显示可移动位置的预制件
    public GameObject attackHighlightPrefab; // 用于显示可攻击位置的预制件
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0.5f, 0.5f, 0); // Cell Gap

    private GameObject[] moveHighlights;
    private Card currentCard;
    private DeckManager deckManager; // 引入DeckManager以更新卡牌状态

    void Start()
    {
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        deckManager = FindObjectOfType<DeckManager>(); // 初始化deckManager引用
        UpdatePosition();
    }

    void UpdatePosition()
    {
        transform.position = CalculateWorldPosition(position); // 更新棋子在场景中的位置
    }

    public void ShowMoveOptions(Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        moveHighlights = new GameObject[4];
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int newPosition = position + directions[i];
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, i, true);
            }
        }
    }

    public void ShowKnightMoveOptions(Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        moveHighlights = new GameObject[8];
        Vector2Int[] directions = 
        {
            new Vector2Int(2, 1), new Vector2Int(2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-2, -1),
            new Vector2Int(1, 2), new Vector2Int(1, -2),
            new Vector2Int(-1, 2), new Vector2Int(-1, -2)
        };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int newPosition = position + directions[i];
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, i, true);
            }
        }
    }

    public void ShowAttackOptions(Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        moveHighlights = new GameObject[4];
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int newPosition = position + directions[i];
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, i, false);
            }
        }
    }

    void HighlightPosition(Vector2Int newPosition, int index, bool isMove)
    {
        Vector3 highlightPosition = CalculateWorldPosition(newPosition);
        GameObject highlightPrefab = isMove ? moveHighlightPrefab : attackHighlightPrefab;
        GameObject highlight = Instantiate(highlightPrefab, highlightPosition, Quaternion.identity);
        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition, isMove);
        moveHighlights[index] = highlight;
    }

    bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    public void Move(Vector2Int newPosition)
    {
        position = newPosition;
        UpdatePosition();
        ClearMoveHighlights();
        ExecuteCurrentCard();
    }

    public void Attack(Vector2Int attackPosition)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Slime slime = monsterObject.GetComponent<Slime>();
            if (slime != null && slime.position == attackPosition)
            {
                slime.TakeDamage(1);
            }
        }
        ClearMoveHighlights();
        ExecuteCurrentCard();
    }

    void ClearMoveHighlights()
    {
        if (moveHighlights != null)
        {
            foreach (var highlight in moveHighlights)
            {
                Destroy(highlight);
            }
        }
    }

    Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        // 计算世界坐标，考虑每个Tile的大小和Cell Gap，并加上偏移量使其居中
        float x = gridPosition.x * (cellSize.x + cellGap.x) + (cellSize.x / 2);
        float y = gridPosition.y * (cellSize.y + cellGap.y) + (cellSize.y / 2);
        return new Vector3(x, y, 0);
    }

    void ExecuteCurrentCard()
    {
        if (currentCard != null)
        {
            deckManager.UseCard(currentCard);
            currentCard = null;
            // 推进回合
            FindObjectOfType<TurnManager>().AdvanceTurn();
        }
    }
}
