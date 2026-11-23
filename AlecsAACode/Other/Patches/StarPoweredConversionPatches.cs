using AlecsAA.AlecsAACode.Enchantments;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace AlecsAA.AlecsAACode.Other.Patches;

public static class StarPoweredConversionPatches
{
    static bool IsStarPowered(CardModel card) => card?.Enchantment is StarPowered;
    static bool IsXCost(CardModel card) => card.CanonicalInstance.EnergyCost.CostsX;
    [ThreadStatic] static bool _readingModifiedEnergy;

    // Adjusts energy value to be 0 if enchanted with Star Powered.
    [HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
    public static class EnergyCostPatch
    {
        static void Postfix(CardModel ____card, ref int __result)
        {
            if (IsStarPowered(____card) && !_readingModifiedEnergy)
                __result = 0;
        }
    }
    
    // For non X-cost cards, uses original energy cost to determine new star cost.
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.CurrentStarCost), MethodType.Getter)]
    public static class CurrentStarCostPatch
    {
        static void Postfix(CardModel __instance, ref int __result)
        {
            int energyCost = __instance.EnergyCost.Canonical;
            if (IsStarPowered(__instance) && !IsXCost(__instance) && energyCost > 0)
            {
                // While resolving energy cost with modifiers, causes a
                // thread pause so energy won't be set to 0 by EnergyCostPatch.
                _readingModifiedEnergy = true;
                energyCost = __instance.EnergyCost.GetWithModifiers(CostModifiers.All);
                _readingModifiedEnergy = false;
                
                // Default star cost for cards with no star cost is -1.
                // This clamps the min up to 0 before adding energyCost.
                __result = Math.Max(0, __instance.BaseStarCost) + energyCost;
            }
        }
    }

    // Adjusts HasEnergyCostX to be false if enchanted with Star Powered.
    [HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.CostsX), MethodType.Getter)]
    public static class HasEnergyCostXPatch
    {
        static void Postfix(CardModel ____card, ref bool  __result)
        {
            if (IsStarPowered(____card) && __result)
            {
                __result = false;
            }
        }
    }

    // Adjusts HasStarCostX to be true if X energy-cost card is enchanted with Star Powered.
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.HasStarCostX), MethodType.Getter)]
    public static class HasStarCostXPatch
    {
        static void Postfix(CardModel __instance, ref bool __result)
        {
            if (IsStarPowered(__instance) && IsXCost(__instance))
            {
                __result = true;
            }
        }
    }
    
    // Has Star Powered cards calling ResolveEnergyXValue() to read from their stars instead.
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.ResolveEnergyXValue))]
    public static class ResolveEnergyXValuePatch
    {
        static bool Prefix(CardModel __instance, ref int __result)
        {
            if (IsStarPowered(__instance) && IsXCost(__instance))
            {
                __result = __instance.ResolveStarXValue();
                return false;
            }
            return true;
        }
    }
    
    // Corrects visual on X-Cost energy cards so the energy cost doesn't continue to appear with 'X'.
    [HarmonyPatch(typeof(NCard), "UpdateEnergyCostVisuals")]
    public static class XEnergyCostVisualsPatch
    {
        static void Postfix(NCard __instance, MegaLabel ____energyLabel)
        {
            CardModel card = __instance.Model;
            if (IsStarPowered(card) && IsXCost(card))
            {
                ____energyLabel.SetTextAutoSize("0");
            }
        }
    }
}