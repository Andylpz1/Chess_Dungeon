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
    public float xshift = -1;
    public float yshift = -1;

    private List<GameObject> moveHighlights = new List<GameObject>(); // 初始化 moveHighlights 列表
    public Card currentCard;
    public DeckManager deckManager; // 引入DeckManager以更新卡牌状态
    public Text goldText;

    public event System.Action OnMoveComplete;

    private GameObject currentHighlight; // 用于存储当前的高亮对象

    void Awake()
    {
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        Debug.Log($"Current Location: {position}");
        deckManager = FindObjectOfType<DeckManager>(); // 初始化deckManager引用
        UpdatePosition();
        UpdateGoldText();
        currentCard = null;
    }

    void Start()
    {
        
    }

    void Update()
    {
        HandleMouseMovement(); // 处理鼠标移动
        HandleMouseClick(); // 处理鼠标点击
    }

    void HandleMouseMovement()
    {
        // 如果当前有选中的卡牌，不执行高亮显示逻辑
        if (currentCard != null)
        {
            ClearCurrentHighlight();
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int gridPosition = CalculateGridPosition(mousePosition);

        // 检查目标位置是否有效，并且是上下左右或斜方向的位置
        if (actions > 0)
        {
            bool isAdjacentToMonster = IsAdjacentToMonster(position);

            if (IsValidPosition(gridPosition) && IsAdjacentOrDiagonal(gridPosition) && !IsBlockedByMonster(gridPosition) && gridPosition != position)
            {
                if (currentHighlight == null || CalculateGridPosition(currentHighlight.transform.position) != gridPosition)
                {
                    // 销毁旧的高亮对象
                    ClearCurrentHighlight();

                    // 调用 HighlightPosition 方法生成新的高亮对象
                    currentHighlight = HighlightPosition(gridPosition, true);
                }
            }
            else if (isAdjacentToMonster && IsValidPosition(gridPosition) && IsAdjacent(gridPosition))
            {
                ClearCurrentHighlight();
                currentHighlight = HighlightPosition(gridPosition, false);
            }
            else
            {
            // 如果鼠标移出了有效位置，移除高亮对象
                ClearCurrentHighlight();
            }
        }
    }

    void HandleMouseClick()
    {
        if (Input.GetMouseButtonDown(0) && actions > 0) // 检查鼠标左键点击并且有可用的行动点
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int targetPosition = CalculateGridPosition(mousePosition);

            // 检查目标位置是否有效，并且是上下左右或斜方向的位置
            if (IsValidPosition(targetPosition) && IsAdjacentOrDiagonal(targetPosition))
            {
                
                ClearCurrentHighlight();
            }
        }   
    }

    private bool IsAdjacentToMonster(Vector2Int position)
    {
        Vector2Int[] adjacentPositions = {
            position + Vector2Int.up,
            position + Vector2Int.down,
            position + Vector2Int.left,
            position + Vector2Int.right
        };

        foreach (var adjacentPosition in adjacentPositions)
        {
            if (IsBlockedByMonster(adjacentPosition))
            {
                return true;
            }
        }

        return false;
    }   


    void ClearCurrentHighlight()
    {
        if (currentHighlight != null)
        {
            Destroy(currentHighlight);
            currentHighlight = null;
        }
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
                //Debug.Log($"player valid Position: {newPosition}");
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

    public GameObject HighlightPosition(Vector2Int newPosition, bool isMove)
    {
        Vector3 highlightPosition = CalculateWorldPosition(newPosition);
        GameObject highlightPrefab = isMove ? moveHighlightPrefab : attackHighlightPrefab;
        GameObject highlight = Instantiate(highlightPrefab, highlightPosition, Quaternion.identity);
        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition, isMove);
        moveHighlights.Add(highlight);  // 使用 List 而不是数组
        return highlight;  // 返回新创建的高亮对象
    }

    public bool IsValidPosition(Vector2Int position)
    {
        return position.x >= 0 && position.x < boardSize && position.y >= 0 && position.y < boardSize;
    }

    private static bool IsBlockedByMonster(Vector2Int position)
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(position))
            {
                return true;
            }
        }

        return false;
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
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPosition))
            {
                monster.TakeDamage(1);
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
                Monster monster = monsterObject.GetComponent<Monster>();
                if (monster != null && monster.IsPartOfMonster(attackPosition))
                {
                    monster.TakeDamage(1);
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

    public Vector2Int CalculateGridPosition(Vector3 worldPosition)
    {
        // 通过减去偏移量来修正网格坐标的计算
        int x = Mathf.FloorToInt((worldPosition.x) / cellSize.x - xshift );
        int y = Mathf.FloorToInt((worldPosition.y) / cellSize.y - yshift );

        return new Vector2Int(x, y);
    }


    public bool IsAdjacent(Vector2Int targetPosition)
    {
        Vector2Int delta = targetPosition - position;
        // 判断是否仅在水平方向或垂直方向上相邻
        return (Mathf.Abs(delta.x) == 1 && delta.y == 0) || (Mathf.Abs(delta.y) == 1 && delta.x == 0);
    }

    public bool IsAdjacentOrDiagonal(Vector2Int targetPosition)
    {
        Vector2Int delta = targetPosition - position;
        return Mathf.Abs(delta.x) <= 1 && Mathf.Abs(delta.y) <= 1;
    }

    public void ExecuteCurrentCard()
    {
        if (currentCard == null)
        {
            actions -= 1;
            if (actions > 0)
            {
                FindObjectOfType<TurnManager>().MoveCursor();
                FindObjectOfType<TurnManager>().UpdateActionText();
            } 
            // 推进回合
            if (actions == 0) 
            {
                FindObjectOfType<TurnManager>().UpdateActionText();   
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
            }  
        }
        if (currentCard != null)
        {
            deckManager.UseCard(currentCard);

            if (!currentCard.isQuick)
            {
                actions -= 1;
                if (actions > 0)
                {
                    FindObjectOfType<TurnManager>().MoveCursor();
                    FindObjectOfType<TurnManager>().UpdateActionText();
                }
            }
            if (currentCard.isPartner)
            {
                for (int i = 0; i < deckManager.deck.Count; i++)
                {
                    Card c = deckManager.deck[i];
                    if (c.isPartner)
                    {
                        deckManager.DrawCardAt(i); // 直接调用 DrawCardAt 方法来抽取特定位置的牌
                        break;
                    }
                }
            }
            currentCard = null;
            // 推进回合
            if (actions == 0) 
            {
                FindObjectOfType<TurnManager>().UpdateActionText();   
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
            }
        }
    }

    public void DisableNonQuickCardButtons()
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
