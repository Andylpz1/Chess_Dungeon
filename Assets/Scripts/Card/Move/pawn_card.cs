using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pawn_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    protected Card card;
    protected DeckManager deckManager;
    protected Button button;
    protected Text buttonText;
    public Player player;
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
            //buttonText.text = "Pawn";
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
                MoveHelper.ShowPawnMoveOptions(player, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            //hintManager.ShowHint("P移动", transform.position, card.GetSprite());
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            //hintManager.HideHint();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ShowCardDescription();
        }
    }

    protected void ShowCardDescription()
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
