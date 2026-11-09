using AlecsAA.AlecsAACode.Cards.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace AlecsAA.AlecsAACode.Other.Patches;

[HarmonyPatch(typeof(Claw), "OnPlay")]
public static class ClawBuffPatch
{
    static void Postfix(Claw __instance, ref Task __result)
    {
        __result = AfterOnPlayPatch(__instance, __result);
    }
    private static async Task AfterOnPlayPatch(Claw claw, Task original)
    {
        await original;
        
        decimal baseValue = claw.DynamicVars["Increase"].BaseValue;
        
        // Original OnPlay evaluates damage increase for all Claw, so this will search for all other cards with Claw tag
        // that still need to be evaluated.
        var otherClaws = claw.Owner.PlayerCombatState.AllCards
            .Where(c => c is not Claw && c.Tags.Contains(CustomEnums.Claw)).ToList();
        
        foreach (CardModel c in otherClaws)
        {
            if (c is IBuffableFromClawPlay buffable)
            {
                //Stores damage increase so damage info isn't lost on card downgrade.
                buffable.BuffFromClawPlay(baseValue);
            }
        }
    }
}