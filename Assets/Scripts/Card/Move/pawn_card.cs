using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class pawn_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;
    public Player player;
    public HintManager hintManager; // 引用HintManager

    private Vector2Int[] pawnDirections = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        if (hintManager == null)
        {
            Debug.LogError("HintManager not found in the scene.");
        }
    }

    public void Initialize(Card card, DeckManager deckManager)
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

    private void OnClick()
    {
        if (card != null)
        {
            if (player.currentCard == card)
            {
                player.DeselectCurrentCard();
            }
            else
            {
                player.ShowMoveOptions(pawnDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("P移动", transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
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
