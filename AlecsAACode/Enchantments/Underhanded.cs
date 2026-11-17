using AlecsAA.AlecsAACode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AlecsAA.AlecsAACode.Enchantments;

public sealed class Underhanded : AlecsAAEnchantment
{
    public override bool ShowAmount => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CardKeyword.Sly)];
    
    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }
    
    protected override void OnEnchant()
    {
        Card.AddKeyword(CardKeyword.Sly);
    }
}