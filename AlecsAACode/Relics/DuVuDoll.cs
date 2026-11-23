using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace AlecsAA.AlecsAACode.Relics;

[Pool(typeof(EventRelicPool))]
public class DuVuDoll() : AlecsAARelic
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>(1m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is CombatRoom)
        {
            int cursesCount = Owner.Deck.Cards.Count(c => c.Type == CardType.Curse);
            if (cursesCount == 0)
            {
                return;
            }
            
            Flash();
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Owner.Creature, DynamicVars.Strength.BaseValue * cursesCount, base.Owner.Creature, null);
        }
    }
}