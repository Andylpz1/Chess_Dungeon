using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class potion_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    public Player player;
    private Text buttonText;
    private TurnManager turnManager;

    public HintManager hintManager; // 引用HintManager

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    void Start()
    {
        hintManager = FindObjectOfType<HintManager>();
        turnManager = FindObjectOfType<TurnManager>();
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
            //buttonText.text = "potion";
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
                player.currentCard = card;
                turnManager.AddAction();
                player.ExecuteCurrentCard();
            }
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("上下左右攻击", transform.position);
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

