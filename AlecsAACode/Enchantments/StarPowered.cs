using AlecsAA.AlecsAACode.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace AlecsAA.AlecsAACode.Enchantments;

public class StarPowered : AlecsAAEnchantment
{
    public override bool ShowAmount => false;

    // Variable declared so {Stars} placeholder for star icon is accessible in localization file
    protected override IEnumerable<DynamicVar> CanonicalVars => [new StarsVar(1)];
    
    public override bool CanEnchant(CardModel card)
    {
        return card.EnergyCost.Canonical > 0 || card.EnergyCost.CostsX;
    }
}