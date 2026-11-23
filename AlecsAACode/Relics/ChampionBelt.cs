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

[Pool(typeof(IroncladRelicPool))]
public class ChampionBelt() : AlecsAARelic
{
    private const string _heavyAmountKey = "HeavyAmount";
    
    public override RelicRarity Rarity => RelicRarity.Shop;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => 
    [
        new CardsVar(2),
        new DynamicVar("HeavyAmount", 2m)
    ];

    public override bool HasUponPickupEffect => true;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<Heavy>(DynamicVars["HeavyAmount"].IntValue);
    
    public override async Task AfterObtained()
    {
        CardSelectorPrefs prefs =
            new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 0, DynamicVars.Cards.IntValue)
            {
                Cancelable = false,
                RequireManualConfirmation = true
            };
        Heavy canonicalEnchantment = ModelDb.Enchantment<Heavy>();
        foreach (CardModel item in await CardSelectCmd.FromDeckForEnchantment(Owner, canonicalEnchantment, DynamicVars["HeavyAmount"].IntValue, prefs))
        {
            CardCmd.Enchant(canonicalEnchantment.ToMutable(), item, DynamicVars["HeavyAmount"].IntValue);
            CardCmd.Preview(item);
        }
    }
}