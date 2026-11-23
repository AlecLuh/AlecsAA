using AlecsAA.AlecsAACode.Cards;
using AlecsAA.AlecsAACode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace AlecsAA.AlecsAACode.Enchantments;

public sealed class Handy : AlecsAAEnchantment
{
    public override bool HasExtraCardText => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Flick>();
    
    public override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay? cardPlay)
    {
        Player owner = Card.Owner;
        await CardPileCmd.AddGeneratedCardToCombat(Card.CombatState.CreateCard<Flick>(owner), PileType.Hand, owner);
    }
}