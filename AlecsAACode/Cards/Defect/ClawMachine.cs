using AlecsAA.AlecsAACode.Cards.Interfaces;
using AlecsAA.AlecsAACode.Other;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace AlecsAA.AlecsAACode.Cards.Defect;

[Pool(typeof(DefectCardPool))]
public class ClawMachine() : AlecsAACard(2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy), IBuffableFromClawPlay
{
    protected override HashSet<CardTag> CanonicalTags => [CustomEnums.Claw];
    private decimal _extraDamageFromClawPlays;

    public decimal ExtraDamageFromClawPlays
    {
        get => _extraDamageFromClawPlays;
        set
        {
            AssertMutable();
            _extraDamageFromClawPlays = value; 
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Move),
        new RepeatVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .FromCard(this, play)
            .TargetingRandomOpponents(CombatState)
            .WithHitVfxNode((Func<Creature, Node2D>) (t => (Node2D) NScratchVfx.Create(t, true)))
            .Execute(choiceContext);
        
        CardPile pile = PileType.Draw.GetPile(Owner);
        var cardModels = pile.Cards.Where(Filter).ToList();
        if (cardModels.Any())
        {
            CardModel card = cardModels.StableShuffle(Owner.RunState.Rng.CombatCardSelection).First();
            await CardPileCmd.Add(card, PileType.Hand);
        }
    }

    private static bool Filter(CardModel card)
    {
        bool flag1 = card.EnergyCost.GetWithModifiers(CostModifiers.All) == 0 && !card.EnergyCost.CostsX;
        if (!flag1)
        {
            return flag1;
        }
        bool flag2 = card.Type is CardType.Attack or CardType.Skill or CardType.Power;
        return flag2;
    }
    
    protected override void OnUpgrade() => this.DynamicVars.Repeat.UpgradeValueBy(1);
    
    protected override void AfterDowngraded()
    {
        base.AfterDowngraded();
        DynamicVars.Damage.BaseValue += ExtraDamageFromClawPlays;
    }
    
    public void BuffFromClawPlay(decimal extraDamage)
    {
        DynamicVars.Damage.BaseValue += extraDamage;
        ExtraDamageFromClawPlays += extraDamage;
    }
}