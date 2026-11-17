using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace AlecsAA.AlecsAACode.Cards;

[Pool(typeof(SilentCardPool))]
public class Freefall() : AlecsAACard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        List<CardModel> cards = PileType.Draw.GetPile(Owner).Cards.ToList();
        await DiscardAndTrigger(choiceContext, cards);
    }

    private async Task DiscardAndTrigger(PlayerChoiceContext choiceContext, List<CardModel> cards)
    {
        if (CombatManager.Instance.IsOverOrEnding || cards.Count == 0)
        {
            return;
        }
        
        ICombatState combatState = CombatState ?? Owner.Creature.CombatState;
        List<CardModel> slyCards = new List<CardModel>();
        CardPile discardPile = PileType.Discard.GetPile(Owner);

        foreach (CardModel card in cards)
        {
            CardPile? pile = card.Pile;
            if (card.IsSlyThisTurn)
            {
                slyCards.Add(card);
            }

            await CardPileCmd.Add(card, discardPile);
            CombatManager.Instance.History.CardDiscarded(combatState, card);
            await Hook.AfterCardDiscarded(combatState, choiceContext, card);
        }
        
        foreach (CardModel item in slyCards)
        {
            await CardCmd.AutoPlay(choiceContext, item, null, AutoPlayType.SlyDiscard);
        }
    }
    
    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}