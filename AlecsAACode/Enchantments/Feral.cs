using AlecsAA.AlecsAACode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace AlecsAA.AlecsAACode.Enchantments;

public class Feral : AlecsAAEnchantment
{
    private bool _usedThisTurn;
    
    public override bool ShowAmount => false;

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }

    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources, CardLocation location)
    {
        if (card != Card || card.IsDupe || _usedThisTurn)
        {
            return location;
        }
        // Checks that other effects like FeralPower didn't return card to hand first.
        if (location.pileType == PileType.Hand)
        {
            return location;
        }
        
        location.pileType = PileType.Hand;
        location.position = CardPilePosition.Top;
        location.player = card.Owner;
        return location;
    }
    
    public override Task AfterModifyingCardPlayResultLocation(CardModel card, CardLocation location)
    {
        if (card == Card)
        {
            _usedThisTurn = true;
        }
        return Task.CompletedTask;
    }
    
    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Card.Owner.Creature))
        {
            _usedThisTurn = false;
        }
        return Task.CompletedTask;
    }
}