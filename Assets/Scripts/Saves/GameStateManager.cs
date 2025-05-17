using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    private void Awake()
    {
        // 单例 & 保留跨场景
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 订阅场景加载完成事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // 注销事件，防止内存泄漏
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 这个就是你要“迁移”的恢复调用
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene,
                   UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        if (scene.name != "GameScene")
            return;

        // 仅当从 LevelNode 进来时，才恢复存档
        bool fromNode = PlayerPrefs.GetInt("IsLevelNode", 0) == 1;
        PlayerPrefs.SetInt("IsLevelNode", 0);
        PlayerPrefs.Save();

        if (fromNode && SaveSystem.GameSaveExists())
        {
            var gameData = SaveSystem.LoadGame();
            if (gameData != null)
            {
                GameManager.Instance.currentGameData = gameData;
                GameManager.Instance.LoadGameData(gameData);
                Debug.Log("🎯 GameStateManager: 从关卡节点恢复存档");
            }
            else
            {
                Debug.LogWarning("⚠️ GameStateManager: 存档损坏");
            }
        }
    }

    // 如果以后要处理 Continue、新游戏分支，也可以加方法：  
    // public void Continue() { … LoadScene("GameScene") … }
    // public void StartNewGame(int lvl) { … }
}
