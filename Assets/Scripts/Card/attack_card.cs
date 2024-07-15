using UnityEngine;
using UnityEngine.UI;

public class attack_card : MonoBehaviour, CardButton
{
    private Card card;
    private DeckManager deckManager;
    private Button button;
    public Player player;
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

    private void OnClick()
    {
        if (card != null)
        {
            player.ShowAttackOptions(card);
        }
        else
        {
            Debug.LogError("Card is null in attack_card.OnClick");
        }
    }

    public Card GetCard()
    {
        return card;
    }
}

