using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class spear_card : MonoBehaviour, CardButton, IPointerEnterHandler, IPointerExitHandler
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    public Player player;
    private Text buttonText;
    Vector2Int[] spearDirections = 
    { 
        new Vector2Int(0, 1), 
        new Vector2Int(0, 2), 
        new Vector2Int(1, 0),
        new Vector2Int(2, 0),  
        new Vector2Int(-1, 0),
        new Vector2Int(-2, 0),  
        new Vector2Int(0, -1),
        new Vector2Int(0, -2)
    };

    public HintManager hintManager; // 引用HintManager

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
            //buttonText.text = "Blade";
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
                player.ShowAttackOptions(spearDirections, card);
            }
        }
        else
        {
            Debug.LogError("Card is null in spear_card.OnClick");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("上下左右两格攻击", transform.position);
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
