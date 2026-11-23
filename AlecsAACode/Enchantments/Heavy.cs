using AlecsAA.AlecsAACode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace AlecsAA.AlecsAACode.Enchantments;

public class Heavy : AlecsAAEnchantment
{
    public override bool ShowAmount => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    public override bool CanEnchantCardType(CardType cardType)
    {
        return cardType == CardType.Attack;
    }

    public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
    {
        if (!props.IsPoweredAttack())
        {
            return 0m;
        }
        return Card.Owner.Creature.GetPowerAmount<StrengthPower>() * Amount;
    }
}