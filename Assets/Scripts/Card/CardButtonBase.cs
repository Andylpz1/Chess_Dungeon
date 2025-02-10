using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private Transform originalParent;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    protected virtual void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        turnManager = FindObjectOfType<TurnManager>();
        monsterManager = FindObjectOfType<MonsterManager>();
        player = FindObjectOfType<Player>();
    }

    public virtual void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;

        if (buttonText != null)
        {
            //buttonText.text = card.GetName();
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
        }
    }

    protected abstract void OnClick();

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (card == null) return;

        isDragging = true;
        originalPosition = transform.position;
        originalParent = transform.parent;

        // 将卡牌临时移动到 Canvas 顶层
        transform.SetParent(GameObject.Find("Canvas").transform, true);

        // 如果是普通卡牌（移动卡或攻击卡），立即调用 OnClick 显示目标位置
        if (card.cardType == CardType.Move || card.cardType == CardType.Attack)
        {
            OnClick();  // 显示目标位置
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card == null) return;

        // 将屏幕坐标转换为世界坐标
        RectTransform canvasRectTransform = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, Camera.main, out worldPosition);

        // 更新卡牌位置
        transform.position = worldPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card == null) return;

        isDragging = false;

        // 检查释放位置
        Vector3 releasePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int gridPosition = player.CalculateGridPosition(releasePosition);
        

        if (card.cardType == CardType.Special)
        {
            //OnClick();  // 特殊卡拖拽到棋盘上时触发效果
        }
        else if (player.IsValidPosition(gridPosition) && IsOverHighlightedPosition(gridPosition))
        {
            Debug.Log("haha");
            // 如果是移动卡，执行移动；如果是攻击卡，执行攻击
            if (card.cardType == CardType.Move)
            {
                Debug.Log("haha");
                player.Move(gridPosition);
            }
            else if (card.cardType == CardType.Attack)
            {
                player.Attack(gridPosition);
            }

            deckManager.UseCard(card);  // 打出卡牌
        }
        else
        {
            // 无效位置，恢复到原位置
            transform.position = originalPosition;
        }

        // 清除高亮
        player.ClearMoveHighlights();
        // 恢复父对象
        transform.SetParent(originalParent, true);
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
}
