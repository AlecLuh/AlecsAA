using AlecsAA.AlecsAACode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace AlecsAA.AlecsAACode.Relics;

[Pool(typeof(SilentRelicPool))]
public class WristBlade() : AlecsAARelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        .. HoverTipFactory.FromEnchantment<Underhanded>(),
        HoverTipFactory.FromKeyword(CardKeyword.Sly)
    ];
    
    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };
        Underhanded canonicalEnchantment = ModelDb.Enchantment<Underhanded>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalEnchantment, 1, prefs))
        {
            CardCmd.Enchant(canonicalEnchantment.ToMutable(), item, 1m);
            CardCmd.Preview(item);
        }
    }
}