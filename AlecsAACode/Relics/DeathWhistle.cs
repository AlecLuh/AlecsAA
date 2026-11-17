using AlecsAA.AlecsAACode.Cards;

using AlecsAA.AlecsAACode.Enchantments;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace AlecsAA.AlecsAACode.Relics;

[Pool(typeof(NecrobinderRelicPool))]
public class DeathWhistle() : AlecsAARelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];
    
    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        .. HoverTipFactory.FromCardWithCardHoverTips<Flick>(),
        .. HoverTipFactory.FromEnchantment<Handy>()
    ];

    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };
        Handy canonicalEnchantment = ModelDb.Enchantment<Handy>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalEnchantment, 1, prefs))
        {
            CardCmd.Enchant(canonicalEnchantment.ToMutable(), item, 1m);
            CardCmd.Preview(item);
        }
    }
}