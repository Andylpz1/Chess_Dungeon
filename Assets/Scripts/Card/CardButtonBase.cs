using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public abstract class CardButtonBase : MonoBehaviour, CardButton, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    protected Card card;
    protected DeckManager deckManager;
    protected Button button;
    protected Text buttonText;
    protected Player player;
    protected HintManager hintManager;
    protected TurnManager turnManager;
    public MonsterManager monsterManager;
    private Vector3 originalPosition;
    private bool isDragging = false;
    protected bool canDrag = true;
    private Transform originalParent;

    private GameObject aimPointer;  // 瞄准指针
    private static GameObject aimPointerInstance;
    private Image cardImage; 
    private RectTransform canvasRectTransform;
    private Transform upgradeEffectTransform;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
        canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        cardImage = GetComponent<Image>();
    }

    protected virtual void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        turnManager = FindObjectOfType<TurnManager>();
        monsterManager = FindObjectOfType<MonsterManager>();
        player = FindObjectOfType<Player>();

        // 如果全局的 AimPointer 没有实例化，就创建一个
        if (aimPointerInstance == null)
        {
            GameObject pointerPrefab = Resources.Load<GameObject>("Prefabs/UI/AimPointer");
            aimPointerInstance = Instantiate(pointerPrefab);
            aimPointerInstance.SetActive(false);
            aimPointerInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        aimPointer = aimPointerInstance;
    }

    public virtual void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;
        upgradeEffectTransform = transform.Find("UpgradeEffect");

        if (buttonText != null)
        {
            //buttonText.text = card.GetName();
        }

        if (button != null)
        {
            //抛弃点击方法
            //button.onClick.AddListener(() => OnClick()); 
        }
    }

    protected abstract void OnClick();

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = transform.position;
        originalParent = transform.parent;
        if (card == null || !canDrag) 
        {
            transform.position = originalPosition;
            return; 
        }

        isDragging = true;
        

        // 将卡牌临时移动到 Canvas 顶层
        transform.SetParent(canvasRectTransform, true);

        

        // 如果是普通卡牌（移动卡或攻击卡），立即调用 OnClick 显示目标位置
        if (card.cardType == CardType.Move || card.cardType == CardType.Attack)
        {
            if (cardImage != null) cardImage.enabled = false;
            // 激活瞄准指针
             if (upgradeEffectTransform != null)
        {
            upgradeEffectTransform.gameObject.SetActive(false);
        }
            aimPointer.SetActive(true);
            UpdateAimPointerPosition(eventData);
            OnClick();  // 显示目标位置
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card == null || !canDrag) 
        {
            transform.position = originalPosition;
            return; 
        }

        // 更新卡牌位置
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, Camera.main, out worldPosition);
        transform.position = worldPosition;

        // 更新瞄准指针的位置
        UpdateAimPointerPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card == null || !canDrag) 
        {
            transform.position = originalPosition;
            return; 
        }

        isDragging = false;

        // 检查释放位置
        Vector3 releasePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int gridPosition = player.CalculateGridPosition(releasePosition);

        if (card.cardType == CardType.Special) 
        {
            if (1==0)
            {
                // 没拖到合法位置 → 不执行任何操作，相当于取消释放
                transform.position = originalPosition;
            }
            else
            {
                OnClick(); // 如果拖到合法位置才释放
            }
        }
        if (player.IsValidPosition(gridPosition) && IsOverHighlightedPosition(gridPosition))
        {
            // 如果是移动卡，执行移动；如果是攻击卡，执行攻击
            // 如果是 FlailCard，则触发多目标攻击
            if (card is FlailCard flailCard)
            {
                Debug.Log($"Performing flail attack at position {gridPosition}");
                List<Vector2Int> attackPositions = flailCard.GetAttackPositions(player.position, gridPosition, player.boardSize);
            
                // 打印调试信息
                foreach (var pos in attackPositions)
                {
                    Debug.Log($"Flail attack target: {pos}");
                }
            
                player.MultipleAttack(attackPositions.ToArray());
            }
            else if (card.cardType == CardType.Move)
            {
                player.Move(gridPosition);
            }
            else if (card.cardType == CardType.Attack)
            {
                player.Attack(gridPosition);
            }

        }
        else
        {
            // 无效位置，恢复到原位置
            if (cardImage != null) cardImage.enabled = true;
            transform.position = originalPosition;
        }

        // 清除高亮
        player.ClearMoveHighlights();
        // 恢复父对象
        transform.SetParent(originalParent, true);
        // 恢复升级特效显示
        if (card.cardType == CardType.Move || card.cardType == CardType.Attack)
        {
            if (upgradeEffectTransform != null && card.IsUpgraded())
            {
                upgradeEffectTransform.gameObject.SetActive(true);
            }
        }

        player.DeselectCurrentCard();

        // 隐藏瞄准指针
        aimPointer.SetActive(false);
    }

    private void UpdateAimPointerPosition(PointerEventData eventData)
    {
        // 将鼠标位置更新到瞄准指针上
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, Camera.main, out worldPosition);
        aimPointer.transform.position = worldPosition;
        
    }

    private bool IsOverHighlightedPosition(Vector2Int position)
    {
        // 检查是否有对应的 highlight 存在于该位置
        foreach (GameObject highlight in player.moveHighlights)
        {
            MoveHighlight moveHighlight = highlight.GetComponent<MoveHighlight>();
            if (moveHighlight != null && moveHighlight.position == position)
            {
                return true;
            }
        }
        return false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowCardDescription();
        }
    }

    public void ShowCardDescription()
    {
        if (hintManager != null && card != null)
        {
            if (hintManager.IsHintVisible())
            {
                hintManager.HideHint();
            }
            else
            {
                hintManager.ShowHint(card.GetDescription(), transform.position, card.GetSprite());
            }
        }
    }

    public Card GetCard()
    {
        return card;
    }

    public virtual void SetDraggable(bool draggable)
    {
        canDrag = draggable;
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
}
