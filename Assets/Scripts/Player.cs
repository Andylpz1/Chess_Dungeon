using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
    public Vector2Int position; // 棋子在棋盘上的位置
    public int boardSize = 8;   // 棋盘大小
    public GameObject moveHighlightPrefab; // 用于显示可移动位置的预制件
    public GameObject attackHighlightPrefab; // 用于显示可攻击位置的预制件
    public Vector3 cellSize = new Vector3(1, 1, 0); // 每个Tile的大小
    public Vector3 cellGap = new Vector3(0, 0, 0); // Cell Gap
    public int gold = 1000; // 金币数量
    public int actions = 3; //行动点
    public int health = 3; // 玩家初始血量
    public List<string> deck;
    public List<Relic> relics;
    public int damage = 1; //默认伤害
    public int cardsUsedThisTurn = 0; //本回合使用的卡牌数量
    public Text healthText; 
    public bool isCharged = false; // 是否处于充能状态
    public Text energyStatusText;

    public int CurrentLevel;

    // 存储 ActivatePoint 和 DeactivatePoint 的位置信息
    public List<Vector2Int> activatePointPositions = new List<Vector2Int>();
    public List<Vector2Int> deactivatePointPositions = new List<Vector2Int>();

    //棋盘偏移量
    public float xshift = -1;
    public float yshift = -1;

    public List<GameObject> moveHighlights = new List<GameObject>(); // 初始化 moveHighlights 列表
    public Card currentCard;
    public DeckManager deckManager; // 引入DeckManager以更新卡牌状态
    public MonsterManager monsterManager;
    public LocationManager locationManager;
    public Text goldText;

    public event System.Action OnMoveComplete;

    private GameObject currentHighlight; // 用于存储当前的高亮对象

    public delegate void CardPlayed(Card card);
    public event CardPlayed OnCardPlayed;


    public bool vineEffectActive = false; 
    public LayerMask attackHighlightLayer;
    public LayerMask moveHighlightLayer;

    private Animator animator;
    public GameObject attackEffectPrefab;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        monsterManager = FindObjectOfType<MonsterManager>(); // 初始化deckManager引用
        position = new Vector2Int(boardSize / 2, boardSize / 2); // 初始化棋子位置到棋盘中央
        //Debug.Log($"Current Location: {position}");
        deckManager = FindObjectOfType<DeckManager>(); // 初始化deckManager引用
        LocationManager locationManager = FindObjectOfType<LocationManager>(); // 初始化locationManager引用
        animator = GetComponent<Animator>(); //初始化animator
        UpdatePosition();
        UpdateGoldText();
        currentCard = null;
    }

    void Start()
    {
        UpdateHealthText();
        //position = monsterManager.GetEmptyPosition();
        //UpdatePosition();

        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.OnGameStart(this);
        }
    }

    void Update()
    {
        //HandleMouseMovement(); // 处理鼠标移动
        //HandleMouseClick(); // 处理鼠标点击
        HandleMoveHighlightClick(); 
        HandleAttackHighlightClick(); 
        CheckForMonsterCollision();
    }

    public void HandleAttackHighlightClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            // 使用 Raycast 仅检测 AttackHighlight Layer
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero, Mathf.Infinity, attackHighlightLayer);
            if (hit.collider != null)
            {
                MoveHighlight highlight = hit.collider.GetComponent<MoveHighlight>();
                if (highlight != null && !highlight.isMove)
                {
                    Attack(highlight.position); // 触发攻击逻辑
                }
            }
        }
    }

    public void HandleMoveHighlightClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            // 使用 Raycast 仅检测 MoveHighlight Layer
            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero, Mathf.Infinity, moveHighlightLayer);
            if (hit.collider != null)
            {
                MoveHighlight highlight = hit.collider.GetComponent<MoveHighlight>();
                if (highlight != null && highlight.isMove)  // 确保是一个移动高亮
                {
                    Debug.Log($"[MoveClick] Moving player to {highlight.position}");
                    Move(highlight.position);  // 调用玩家的 Move 方法
                }
                else
                {
                    Debug.LogWarning("[MoveClick] Detected highlight, but it's not a move highlight.");
                }
            }
            else
            {
                Debug.LogWarning("[MoveClick] No move highlight detected at click position.");
            }
        }
    }




    
    private void CheckForMonsterCollision()
    {
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(position))
            {
                TakeDamage(1); // 玩家受到伤害
                Vector2Int newPosition = monsterManager.GetEmptyPosition(); // 获取新位置
                if (newPosition != new Vector2Int(-1, -1))
                {
                    position = newPosition;
                    UpdatePosition(); // 更新玩家位置
                }
                break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        UpdateHealthText();
        if (health <= 0)
        {
            //("Player has died.");
            // 可在此实现游戏结束逻辑
        }
    }

    public void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + health.ToString();
        }
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

        if (!isMove)
        {
            highlight.layer = LayerMask.NameToLayer("AttackHighlight"); // 设置专属 Layer
        }
        else
        {
            highlight.layer = LayerMask.NameToLayer("MoveHighlight");
        }
        

        highlight.GetComponent<MoveHighlight>().Initialize(this, newPosition, isMove);
        moveHighlights.Add(highlight);
        return highlight;
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

    private bool IsBlockedByLocation(Vector2Int position)
    {
        // 检查该位置是否被不可进入的位置占据
        return locationManager.IsNonEnterablePosition(position);
    }


    public void Move(Vector2Int newPosition)
    {
        position = newPosition;
        UpdatePosition();
        ClearMoveHighlights();

        // 调用新方法来检查并处理 ActivatePoint 和 DeactivatePoint
        CheckAndHandlePoints(newPosition);

        ExecuteCurrentCard();
        // 移动完成后触发事件
        OnMoveComplete?.Invoke();
    }

    public void UpdateEnergyStatus()
    {
        if (energyStatusText != null)
        {
            // Update the text based on the isCharged status
            energyStatusText.text = isCharged ? "ON" : "OFF";
        }
        else
        {
            //Debug.LogError("Energy status text component is not assigned.");
        }
    }

    public void CheckAndHandlePoints(Vector2Int newPosition)
    {

        // 检查玩家是否移动到了ActivatePoint
        if (activatePointPositions.Contains(newPosition))
        {
            Charge();
            //Debug.Log("Player is now charged.");
        }

        // 检查玩家是否移动到了DeactivatePoint
        if (deactivatePointPositions.Contains(newPosition))
        {
            if (isCharged)
            {
                //Debug.Log("Player is at DeactivatePoint, triggering Exhaust.");
                Decharge();
            }
        }
    }

    public void Charge() {
        isCharged = true;
        UpdateEnergyStatus();
    }

    public void Decharge() {
        deckManager.Exhaust();
        isCharged = false;
        UpdateEnergyStatus();
    }




    public void Attack(Vector2Int attackPosition)
    {
        Vector3 worldPosition = CalculateWorldPosition(attackPosition);
        GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
        // 播放攻击动画
        Destroy(effectInstance, 0.1f);
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (GameObject monsterObject in monsters)
        {
            Monster monster = monsterObject.GetComponent<Monster>();
            if (monster != null && monster.IsPartOfMonster(attackPosition))
            {
                monster.TakeDamage(damage);
            }
        }
        damage = 1;
        ClearMoveHighlights();
        ExecuteCurrentCard();
    }


    public void MultipleAttack(Vector2Int[] attackPositions)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (Vector2Int attackPosition in attackPositions)
        {
            // 在每个攻击坐标生成攻击特效
            Vector3 worldPosition = CalculateWorldPosition(attackPosition);
            GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
            Destroy(effectInstance, 0.1f);  // 根据动画时长调整销毁时间
            
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
        if (currentCard.cardType == CardType.Attack)
        {
            ExecuteCurrentCard();
        }
    }

    // 对于bookofpawn等等牌
    public void MultipleSpecial(Vector2Int[] attackPositions)
    {
        // 基于坐标检测 Monster 的存在
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        foreach (Vector2Int attackPosition in attackPositions)
        {
            // 在每个攻击坐标生成攻击特效
            Vector3 worldPosition = CalculateWorldPosition(attackPosition);
            GameObject effectInstance = Instantiate(attackEffectPrefab, worldPosition, Quaternion.identity);
            Destroy(effectInstance, 0.1f);  // 根据动画时长调整销毁时间
            
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
            
            // 如果是攻击卡，执行 OnAttackExecuted 方法
            if (currentCard.cardType == CardType.Attack)
            {
                //Debug.Log("Executing OnAttackExecuted for: ");
                currentCard.OnCardExecuted();   // 只触发攻击卡的特殊效果
            }

            if (currentCard.cardType == CardType.Move) // Assuming MovementCard is a class for movement cards
            {
                if (vineEffectActive)
                {
                    TriggerVineEffect();
                }
            }
            

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

            // Notify listeners that a card has been played
            OnCardUsed(currentCard);
            currentCard = null;
            // 推进回合
            if (actions == 0) 
            {
                //ResetEffectsAtEndOfTurn();
                FindObjectOfType<TurnManager>().UpdateActionText();   
                DisableNonQuickCardButtons();
                //添加回合条变成红色特效
            }
        }
    }

    public void ResetEffectsAtEndOfTurn()
    {
        vineEffectActive = false; // Reset the vine effect after the turn
        cardsUsedThisTurn = 0;
    }

    public void OnCardUsed(Card currentCard)
    {
        cardsUsedThisTurn++;
        OnCardPlayed?.Invoke(currentCard); // Trigger the global event when a card is used
    }

    private void TriggerVineEffect()
    {
        Monster nearestMonster = monsterManager.FindNearestMonster(position, true);

        if (nearestMonster != null)
        {
            nearestMonster.TakeDamage(1);
            //Debug.Log("Vine effect triggered: Dealt 1 damage to " + nearestMonster.name);
        }
        else
        {
            //Debug.Log("No adjacent monsters to damage.");
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
                    // 禁用拖拽
                    CardButtonBase cardButtonBase = monoBehaviour as CardButtonBase;
                    if (cardButtonBase != null)
                    {
                        cardButtonBase.SetDraggable(false);
                    }
                    }
            }
        }

    }

    public void SetDeck(List<Card> deck)
    {
        this.deck = new List<string>();
        foreach (Card card in deck)
        {
            this.deck.Add(card.Id); // Assuming each Card has a `cardName` property
        }
    }

    public void DeselectCurrentCard()
    {
        currentCard = null;
        ClearMoveHighlights();
    }

    // 示例方法：读取卡组
    public List<string> GetDeckNames()
    {
        return new List<string>(deck);
    }

    // 示例方法：加载卡组
    public void LoadDeck(List<string> newDeck)
    {
        deck = new List<string>(newDeck);
        deckManager.UpdateDeckPanel(); 
        deckManager.RefreshCardReferences(this, monsterManager);
    }

    // 示例方法：设置玩家位置
    public void SetPosition(Vector2Int newPosition)
    {
        position = newPosition;
    }

    // 示例方法：设置玩家生命值
    public void SetHealth(int newHealth)
    {
        health = newHealth;
        UpdateHealthText();
    }

    // 示例方法：设置金币
    public void SetGold(int newGold)
    {
        gold = newGold;
    }
}
