using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace AlecsAA.AlecsAACode.Cards;

[Pool(typeof(DefectCardPool))]
public class Vitrify() : AlecsAACard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsUpgraded
        ? [HoverTipFactory.FromOrb<GlassOrb>(), HoverTipFactory.Static(StaticHoverTip.Evoke)]
        : [HoverTipFactory.FromOrb<GlassOrb>()];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        OrbQueue orbQueue = Owner.PlayerCombatState.OrbQueue;
        int orbCount = orbQueue.Orbs.Count;
        
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        for (int i = 0; i < orbCount; i++)
        {
            if (IsUpgraded)
            {
                await OrbCmd.EvokeNext(choiceContext, Owner);
            }
            else
            {
                OrbModel firstOrb = orbQueue.Orbs.First();
                orbQueue.Remove(firstOrb);
                NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.OrbManager?.EvokeOrbAnim(firstOrb);
                firstOrb.RemoveInternal();
            }
        }
        for (int i = 0; i < orbCount; i++)
        { 
            await OrbCmd.Channel<GlassOrb>(choiceContext, Owner);
        }
    }
}