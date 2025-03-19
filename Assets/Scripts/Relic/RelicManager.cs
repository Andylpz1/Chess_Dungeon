using System.Collections.Generic;
using UnityEngine;

public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance;

    public List<Relic> relics = new List<Relic>();
    public Relic moveCardDrawRelic; 
    public Relic HandSizeRelic; 
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 场景切换不会销毁
            Player player = FindObjectOfType<Player>();
            //AcquireRelic(moveCardDrawRelic, player);
            //AcquireRelic(HandSizeRelic, player);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 玩家获得遗物调用这个方法
    public void AcquireRelic(Relic relic, Player player)
    {
        relics.Add(relic);
        relic.OnAcquire(player);
    }

    // 每关卡开始调用
    public void OnGameStart(Player player)
    {
        foreach (var relic in relics)
        {
            relic.OnGameStart(player);
        }
    }
}
