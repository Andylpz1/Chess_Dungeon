using UnityEngine;
using UnityEngine.UI;

public class pawn_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    private Text buttonText;

    void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<Text>();
    }

    public void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;

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
            card.ShowOptions(FindObjectOfType<Player>());
        }
        else
        {
            Debug.LogError("Card is null in pawn_card.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}
