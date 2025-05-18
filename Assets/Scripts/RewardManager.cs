using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class RewardManager : MonoBehaviour
{
    [Header("卡牌奖励")]
    public GameObject rewardPanel;
    public Image card1, card2, card3;
    public Text text1, text2, text3;
    public Button refreshButton, skipButton;

    [Header("遗物奖励")]
    public GameObject relicPanel;
    public Image relicImage1, relicImage2, relicImage3;
    public Text relicText1, relicText2, relicText3;
    public Button relicRefreshButton, relicSkipButton;

    private DeckManager deckManager;
    public GameManager gameManager;
    public bool isRewardPanelOpen = false;

    private List<Card> rewardCards;
    private List<Relic> rewardRelics;
    private bool hasCards;
    private bool hasRelics;

    public event System.Action OnRewardSelectionComplete;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        rewardPanel.SetActive(false);
        relicPanel.SetActive(false);
        deckManager = FindObjectOfType<DeckManager>();

        refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        skipButton.onClick.AddListener(OnSkipButtonClicked);
        relicRefreshButton.onClick.AddListener(OnRelicRefreshButtonClicked);
        relicSkipButton.onClick.AddListener(OnRelicSkipButtonClicked);
    }

    /// <summary>
    /// 启动整个奖励流程：先卡牌后遗物
    /// </summary>
    public void StartRewardProcess()
    {
        // 生成并判断卡牌奖励
        rewardCards = CardPoolManager.GenerateRewardCards();
        hasCards = rewardCards != null && rewardCards.Count > 0;

        // 生成并判断遗物奖励
        rewardRelics = GenerateRelicChoices();
        //hasRelics = rewardRelics != null && rewardRelics.Count > 0;
        hasRelics = false;

        if (hasCards)
            OpenRewardPanel();
        else if (hasRelics)
            OpenRelicPanel();
        else
            EndReward();
    }

    #region 卡牌阶段
    public void OpenRewardPanel()
    {
        isRewardPanelOpen = true;
        // 隐藏遗物面板
        relicPanel.SetActive(false);
        rewardPanel.SetActive(true);

        // 绑定卡牌
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    private void SetCard(Image img, Text txt, Card card)
    {
        img.sprite = card.GetSprite();
        txt.text = card.GetDescription();
        Button btn = img.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnCardSelected(card));
    }

    private void OnCardSelected(Card selectedCard)
    {
        deckManager.deck.Add(selectedCard);
        deckManager.UpdateDeckCountText();
        deckManager.UpdateDeckPanel();

        CloseRewardPanel();

        if (hasRelics)
            OpenRelicPanel();
        else
            EndReward();
    }

    private void OnRefreshButtonClicked()
    {
        rewardCards = CardPoolManager.GenerateRewardCards();
        SetCard(card1, text1, rewardCards[0]);
        SetCard(card2, text2, rewardCards[1]);
        SetCard(card3, text3, rewardCards[2]);
    }

    private void OnSkipButtonClicked()
    {
        CloseRewardPanel();

        if (hasRelics)
            OpenRelicPanel();
        else
            EndReward();
    }

    private void CloseRewardPanel()
    {
        rewardPanel.SetActive(false);
    }
    #endregion

    #region 遗物阶段
    public void OpenRelicPanel()
    {
        isRewardPanelOpen = true;
        // 隐藏卡牌面板
        rewardPanel.SetActive(false);
        relicPanel.SetActive(true);

        SetRelic(relicImage1, relicText1, rewardRelics[0]);
        SetRelic(relicImage2, relicText2, rewardRelics[1]);
        SetRelic(relicImage3, relicText3, rewardRelics[2]);
    }

    private void SetRelic(Image img, Text txt, Relic relic)
    {
        img.sprite = relic.icon;
        txt.text = relic.description;
        Button btn = img.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => OnRelicSelected(relic));
    }

    private void OnRelicSelected(Relic selectedRelic)
    {
        Player player = FindObjectOfType<Player>();
        RelicManager.Instance.AcquireRelic(selectedRelic, player);

        CloseRelicPanel();
        EndReward();
    }

    private void OnRelicRefreshButtonClicked()
    {
        rewardRelics = GenerateRelicChoices();
        SetRelic(relicImage1, relicText1, rewardRelics[0]);
        SetRelic(relicImage2, relicText2, rewardRelics[1]);
        SetRelic(relicImage3, relicText3, rewardRelics[2]);
    }

    private void OnRelicSkipButtonClicked()
    {
        CloseRelicPanel();
        EndReward();
    }

    private void CloseRelicPanel()
    {
        relicPanel.SetActive(false);
    }
    #endregion

    /// <summary>
    /// 结束奖励流程：触发回调、存档、切场景
    /// </summary>
    private void EndReward()
    {
        OnRewardSelectionComplete?.Invoke();
        gameManager.SaveGame();
        SceneManager.LoadScene("LevelSelectionScene");
    }

    /// <summary>
    /// 随机挑选 3 个遗物
    /// </summary>
    private List<Relic> GenerateRelicChoices()
    {
        var all = RelicManager.Instance.availableRelics;
        var candidates = all.FindAll(r => !RelicManager.Instance.relics.Contains(r));
        for (int i = 0; i < candidates.Count; i++)
        {
            int j = Random.Range(i, candidates.Count);
            var tmp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = tmp;
        }
        // 保证至少 3 项
        if (candidates.Count < 3)
        {
            var padded = new List<Relic>(candidates);
            int idx = 0;
            while (padded.Count < 3 && candidates.Count > 0)
            {
                padded.Add(candidates[idx % candidates.Count]);
                idx++;
            }
            candidates = padded;
        }
        return candidates.GetRange(0, Mathf.Min(3, candidates.Count));
    }
}
