using AlecsAA.AlecsAACode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace AlecsAA.AlecsAACode.Relics;

[Pool(typeof(RegentRelicPool))]
public class SolarPanel() : AlecsAARelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<StarPowered>();

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        StarPowered canonicalEnchantment = ModelDb.Enchantment<StarPowered>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalEnchantment, 1, prefs))
        {
            CardCmd.Enchant(canonicalEnchantment.ToMutable(), item, 1m);
            CardCmd.Preview(item);
        }
    }
}