using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace AlecsAA.AlecsAACode.Cards;

[Pool(typeof(NecrobinderCardPool))]
public class Conjure() : AlecsAACard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        var cards = Owner.PlayerCombatState.AllCards.OfType<Soul>()
            .Where(c => c.Pile?.Type is PileType.Draw or PileType.Discard)
            .ToList();
        
        await CardPileCmd.Add(cards, PileType.Hand);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}