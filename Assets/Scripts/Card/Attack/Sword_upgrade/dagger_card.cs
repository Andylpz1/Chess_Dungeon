using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dagger_card : sword_card
{
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        // 你可以在这里添加其他初始化代码
    }

    protected override void OnClick()
    {
        base.OnClick();
        // 你可以在这里添加其他点击事件处理代码
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("快速，sword攻击", transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }
    
}