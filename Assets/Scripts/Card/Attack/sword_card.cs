using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class sword_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    public Player player;
    private Text buttonText;
    Vector2Int[] swordDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    public HintManager hintManager; // 引用HintManager

    protected void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    protected void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        if (hintManager == null)
        {
            Debug.LogError("HintManager not found in the scene.");
        }
    }

    public virtual void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;
        player = FindObjectOfType<Player>();

        if (buttonText != null)
        {
            //buttonText.text = "Attack";
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
        }
    }

    protected virtual void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                player.ShowAttackOptions(swordDirections,card);
            }
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("上下左右攻击", transform.position);
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.HideHint();
        }
    }

    public Card GetCard()
    {
        return card;
    }
}

