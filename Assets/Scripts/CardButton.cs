using UnityEngine;
using UnityEngine.UI;

public class CardButton : MonoBehaviour
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();

        if (button == null)
        {
            Debug.LogError("Button component not found on CardButton prefab.");
        }

        if (buttonText == null)
        {
            Debug.LogError("Text component not found on CardButton prefab.");
        }
    }

    public void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;

        if (buttonText != null)
        {
            if (card.cardType == CardType.Move)
            {
                buttonText.text = card.moveType.ToString();
            }
            else
            {
                buttonText.text = card.cardType.ToString();
            }
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
            card.ShowOptions(FindObjectOfType<Player>());
        }
        else
        {
            Debug.LogError("Card is null in CardButton.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
