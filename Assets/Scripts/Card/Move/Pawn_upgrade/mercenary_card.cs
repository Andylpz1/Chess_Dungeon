using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mercenary_card: pawn_card
{
    private bool hasDrawnPartnerCard = false; // 确保只抓一次牌
    public override void Initialize(Card card, DeckManager deckManager)
    {
        base.Initialize(card, deckManager);
        // 你可以在这里添加其他初始化代码
    }

    protected override void OnClick()
    {
        base.OnClick();
        // 你可以在这里添加其他点击事件处理代码
        MoveHelper.ShowPawnMoveOptions(player, card);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (hintManager != null)
        {
            hintManager.ShowHint("拍档, P移动", transform.position);
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
    }


}
