using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public Vector2Int position; // 棋子在棋盘上的位置
    public int boardSize = 5;   // 棋盘大小
    public GameObject moveHighlightPrefab; // 用于显示可移动位置的预制件
    public GameObject attackHighlightPrefab; // 用于显示可攻击位置的预制件
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap
    public int gold = 1000; // 金币数量
    public int actions = 3; //行动点

    //棋盘偏移量
    public int xshift = 0;
    public float yshift = -0.5f;

    private List<GameObject> moveHighlights = new List<GameObject>(); // 初始化 moveHighlights 列表
    public Card currentCard;
    public DeckManager deckManager; // 引入DeckManager以更新卡牌状态
    public Text goldText;

    public event System.Action OnMoveComplete;

    void Start()
    {
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        Debug.Log($"Current Location: {position}");
        deckManager = FindObjectOfType<DeckManager>(); // 初始化deckManager引用
        UpdatePosition();
        UpdateGoldText();
    }

    void UpdatePosition()
    {
        transform.position = CalculateWorldPosition(position); // 更新棋子在场景中的位置
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold.ToString();
        }
    }

    public void ShowMoveOptions(Vector2Int[] directions, Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        foreach (var direction in directions)
        {
            Vector2Int newPosition = position + direction;
            if (IsValidPosition(newPosition))
            {
                Debug.Log($"player valid Position: {newPosition}");
                HighlightPosition(newPosition, true);
            }
        }
    }

    public void ShowAttackOptions(Vector2Int[] directions, Card card)
    {
        ClearMoveHighlights();
        currentCard = card;

        foreach (var direction in directions)
        {
            Vector2Int newPosition = position + direction;
            if (IsValidPosition(newPosition))
            {
                HighlightPosition(newPosition, false);
            }
        }
    }

    public void HighlightPosition(Vector2Int newPosition, bool isMove)
    {
        Vector3 highlightPosition = CalculateWorldPosition(newPosition);
        GameObject highlightPrefab = isMove ? moveHighlightPrefab : attackHighlightPrefab;
        GameObject highlight = Instantiate(highlightPrefab, highlightPosition, Quaternion.identity);
        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition, isMove);
        moveHighlights.Add(highlight);  // 使用 List 而不是数组
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    public void Move(Vector2Int newPosition)
    {
        position = newPosition;
        UpdatePosition();
        ClearMoveHighlights();
        ExecuteCurrentCard();
        // 移动完成后触发事件
        OnMoveComplete?.Invoke();
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

    public void MultipleAttack(Vector2Int[] attackPositions)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (Vector2Int attackPosition in attackPositions)
        {
            foreach (GameObject monsterObject in monsters)
            {
                Slime slime = monsterObject.GetComponent<Slime>();
                if (slime != null && slime.position == attackPosition)
                {
                    slime.TakeDamage(1);
                }
            }
        }
        ClearMoveHighlights();
        ExecuteCurrentCard();
    }


    public void ClearMoveHighlights()
    {
        if (moveHighlights != null)
        {
            foreach (var highlight in moveHighlights)
            {
                Destroy(highlight);
            }
            moveHighlights.Clear();  // 清空列表
        }
    }

    public Vector3 CalculateWorldPosition(Vector2Int gridPosition)
    {
        // 计算世界坐标，仅考虑每个Tile的大小
        float x = (gridPosition.x + xshift) * cellSize.x + (cellSize.x / 2);
        float y = (gridPosition.y + yshift) * cellSize.y + (cellSize.y / 2);
        return new Vector3(x, y, 0);
    }

    public void ExecuteCurrentCard()
    {
        if (currentCard != null)
        {
            deckManager.UseCard(currentCard);

            if (!currentCard.isQuick)
            {
                actions -= 1;
            }
            currentCard = null;
            if (actions > 0)
            {
                FindObjectOfType<TurnManager>().MoveCursor();
            }
            // 推进回合
            if (actions == 0) 
            {
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
            }
        }
    }

    private void DisableNonQuickCardButtons()
    {
        // 获取所有 MonoBehaviour 并筛选出实现了 CardButton 接口的对象
        MonoBehaviour[] monoBehaviours = FindObjectsOfType<MonoBehaviour>();
    
        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            CardButton cardButton = monoBehaviour as CardButton;
            if (cardButton != null)
            {
                // 获取按钮对应的卡牌
                Card card = cardButton.GetCard();
                if (card != null && !card.isQuick)
                {
                    // 禁用按钮
                    Button button = monoBehaviour.GetComponent<Button>();
                    if (button != null)
                    {
                        button.interactable = false;
                    }
                }
            }
        }
    }

    

    public void DeselectCurrentCard()
    {
        currentCard = null;
        ClearMoveHighlights();
    }
}
