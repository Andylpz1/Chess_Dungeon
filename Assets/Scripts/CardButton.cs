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
        buttonText = GetComponentInChildren<Text>(true);

        if (button == null)
        {
            Debug.LogError("Button component not found on CardButton prefab.");
        }
        else
        {
            Debug.Log("Button component found.");
        }

        if (buttonText == null)
        {
            Debug.LogError("Text component not found in children of CardButton prefab.");
        }
        else
        {
            Debug.Log("Text component found.");
        }
    }

    public void Initialize(Card card, DeckManager deckManager)
    {
        this.card = card;
        this.deckManager = deckManager;

        if (buttonText != null)
        {
            buttonText.text = card.cardType.ToString(); // 设置按钮文本为卡牌类型
            Debug.Log("Button text set to: " + buttonText.text);
        }
        else
        {
            Debug.LogError("Button text is null when initializing CardButton.");
        }

        if (button != null)
        {
            button.onClick.AddListener(() => OnClick());
        }
        else
        {
            Debug.LogError("Button component is null when adding listener.");
        }
    }

    private void OnClick()
    {
        deckManager.UseCard(card, gameObject); // 传递当前按钮引用
    }
}
