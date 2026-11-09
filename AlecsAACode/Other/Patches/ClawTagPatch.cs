using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace AlecsAA.AlecsAACode.Other.Patches;

[HarmonyPatch(typeof(CardModel), "CanonicalTags", MethodType.Getter)]
public static class ClawTagPatch
{
    static void Postfix(CardModel __instance, ref HashSet<CardTag> __result)
    {
        if (__instance is not Claw)
        {
            return;
        }

        __result = new HashSet<CardTag>(__result) { CustomEnums.Claw };
    }
}