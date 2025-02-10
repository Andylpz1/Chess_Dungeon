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

    private GameObject aimPointer;  // 瞄准指针
    private static GameObject aimPointerInstance;
    private Image cardImage; 
    private RectTransform canvasRectTransform;

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
        }

        aimPointer = aimPointerInstance;
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
        transform.SetParent(canvasRectTransform, true);

        // 激活瞄准指针
        aimPointer.SetActive(true);
        UpdateAimPointerPosition(eventData);

        // 如果是普通卡牌（移动卡或攻击卡），立即调用 OnClick 显示目标位置
        if (card.cardType == CardType.Move || card.cardType == CardType.Attack)
        {
            if (cardImage != null) cardImage.enabled = false;
            OnClick();  // 显示目标位置
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (card == null) return;

        // 更新卡牌位置
        Vector3 worldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRectTransform, eventData.position, Camera.main, out worldPosition);
        transform.position = worldPosition;

        // 更新瞄准指针的位置
        UpdateAimPointerPosition(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (card == null) return;

        isDragging = false;

        // 检查释放位置
        Vector3 releasePosition = Camera.main.ScreenToWorldPoint(eventData.position);
        Vector2Int gridPosition = player.CalculateGridPosition(releasePosition);
    
        if (player.IsValidPosition(gridPosition) && IsOverHighlightedPosition(gridPosition))
        {
            // 如果是移动卡，执行移动；如果是攻击卡，执行攻击
            if (card.cardType == CardType.Move)
            {
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
            if (cardImage != null) cardImage.enabled = true;
            transform.position = originalPosition;
        }

        // 清除高亮
        player.ClearMoveHighlights();
        // 恢复父对象
        transform.SetParent(originalParent, true);

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
}
