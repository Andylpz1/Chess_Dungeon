using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class mercenary_card: pawn_card
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
        player.ShowMoveOptions(pawnDirections, card);
        player.OnMoveComplete += DrawPartnerCard;

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

    //抽一张拍档卡
    private void DrawPartnerCard()
    {
        for (int i = 0; i < deckManager.deck.Count; i++)
        {
            Card c = deckManager.deck[i];
            if (c.isPartner)
            {
                deckManager.DrawCardAt(i); // 直接调用 DrawCardAt 方法来抽取特定位置的牌
                break;
            }
        }

        // 取消订阅事件，以防止重复调用
        player.OnMoveComplete -= DrawPartnerCard;
    }
}
