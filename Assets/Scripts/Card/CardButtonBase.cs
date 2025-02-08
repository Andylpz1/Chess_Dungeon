using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class CardButtonBase : MonoBehaviour, CardButton, IPointerClickHandler
{
    protected Card card;
    protected DeckManager deckManager;
    protected Button button;
    protected Text buttonText;
    protected Player player;
    protected HintManager hintManager; // 引用HintManager
    protected TurnManager turnManager; // 引用TurnManager
    public MonsterManager monsterManager;

    protected virtual void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
        Debug.Log("CardButtonBase Awake");
    }

    protected virtual void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        turnManager = FindObjectOfType<TurnManager>();
        monsterManager = GameObject.FindObjectOfType<MonsterManager>();
        player = GameObject.FindObjectOfType<Player>();

        if (hintManager == null)
        {
            Debug.LogError("HintManager not found in the scene.");
        }
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }
        Debug.Log("CardButtonBase Start");
    }

    public virtual void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;

        if (buttonText != null)
        {
            // buttonText.text = "Card";
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
            Debug.Log("Button click listener added.");
        }

        Debug.Log("CardButtonBase Initialize with card: " + (card != null ? card.ToString() : "null"));
    }

    protected abstract void OnClick();

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowCardDescription();
            Debug.Log("Right-click detected, showing card description.");
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
            Debug.Log("Card description shown.");
        }
        else
        {
            Debug.LogError("HintManager or Card is null when trying to show card description.");
        }
    }

    public Card GetCard()
    {
        return card;
    }
    
}
