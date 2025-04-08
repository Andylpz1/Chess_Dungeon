using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RewardManager : MonoBehaviour
{
    public GameObject rewardPanel;         // 奖励面板对象
    public Image card1;                    // 卡牌1对应的 Image
    public Image card2;                    // 卡牌2对应的 Image
    public Image card3;                    // 卡牌3对应的 Image
    public Text text1;                     // 卡牌1描述文本
    public Text text2;                     // 卡牌2描述文本
    public Text text3;                     // 卡牌3描述文本
    public Button refreshButton;           // 刷新卡牌按钮
    public Button skipButton;              // 跳过奖励按钮

    private DeckManager deckManager;       // 对场景内 DeckManager 的引用
    public GameManager gameManager;        // 对 GameManager 的引用
    public bool isRewardPanelOpen = false; // 标记奖励面板是否打开
    private List<Card> rewardCards;        // 当前奖励卡牌列表

    public event System.Action OnRewardSelectionComplete;

    private void Awake()
    {
        // 自动寻找 GameManager
        gameManager = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        rewardPanel.SetActive(false); // 开始时隐藏奖励面板
        deckManager = FindObjectOfType<DeckManager>();
        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
    }

    /// <summary>
    /// 打开奖励面板，显示三张奖励卡牌
    /// </summary>
    public void OpenRewardPanel()
    {
        isRewardPanelOpen = true;
        rewardPanel.SetActive(true);
        // 直接调用 CardPoolManager 生成奖励卡牌
        rewardCards = CardPoolManager.GenerateRewardCards();

        // 将奖励卡牌绑定到对应 UI 上
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    /// <summary>
    /// 设置卡牌的图像和描述，并绑定点击事件
    /// </summary>
    private void SetCard(Image cardImage, Text cardText, Card card)
    {
        cardImage.sprite = card.GetSprite();
        cardText.text = card.GetDescription();
        // 移除之前的监听事件，绑定当前卡牌的选择事件
        cardImage.GetComponent<Button>().onClick.RemoveAllListeners();
        cardImage.GetComponent<Button>().onClick.AddListener(() => OnCardSelected(card));
    }

    /// <summary>
    /// 当玩家选择一张卡牌时调用
    /// </summary>
    private void OnCardSelected(Card selectedCard)
    {
        deckManager.deck.Add(selectedCard); // 将选择的卡牌添加到玩家牌组中
        deckManager.UpdateDeckCountText();
        deckManager.UpdateDeckPanel();

        CloseRewardPanel();
        OnRewardSelectionComplete?.Invoke();
        gameManager.SaveGame();
        SceneManager.LoadScene("LevelSelectionScene");
    }

    /// <summary>
    /// 刷新按钮点击事件：生成新的奖励卡牌
    /// </summary>
    private void OnRefreshButtonClicked()
    {
        rewardCards = CardPoolManager.GenerateRewardCards();
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    /// <summary>
    /// 跳过奖励：关闭面板并继续游戏
    /// </summary>
    private void OnSkipButtonClicked()
    {
        CloseRewardPanel();
        OnRewardSelectionComplete?.Invoke();
        gameManager.SaveGame();
        SceneManager.LoadScene("LevelSelectionScene");
    }

    /// <summary>
    /// 关闭奖励面板
    /// </summary>
    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
    }
}
