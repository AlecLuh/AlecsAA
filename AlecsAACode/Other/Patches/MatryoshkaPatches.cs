using AlecsAA.AlecsAACode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace AlecsAA.AlecsAACode.Other.Patches;

internal static class MatryoshkaState
{
    public static bool IsActive; //Used in multiplayer to determine if a voting round is active for relic selection.
    public static bool HoldersOnlyRound;
    public static List<Player> Holders = new();

    public static void Reset()
    {
        IsActive = false;
        HoldersOnlyRound = false;
        Holders.Clear();
    }
    
}

[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), nameof(TreasureRoomRelicSynchronizer.BeginRelicPicking))]
public static class BeginRelicPickingPatch
{
    static void Postfix(
        IRunState ____runState,
        RelicGrabBag ____sharedGrabBag,
        Rng ____rng,
        List<RelicModel>? ____currentRelics,
        ref int ____roundsLeft)
    {
        MatryoshkaState.Reset();

        if (____currentRelics == null)
        {
            return;
        }
        
        foreach (Player player in ____runState.Players)
        {
            Matryoshka matryoshka = player.Relics.OfType<Matryoshka>().FirstOrDefault(r => !r.IsUsedUp);
            if (matryoshka != null)
            {
                RelicRarity rarity = RelicFactory.RollRarity(____rng);
                RelicModel item = ____sharedGrabBag.PullFromFront(rarity, ____runState) ?? RelicFactory.FallbackRelic;
                MatryoshkaState.Holders.Add(player);
                ____currentRelics.Add(item);

                matryoshka.TimesUsed++;
                MatryoshkaState.Holders.Add(player);
            }
        }

        if (MatryoshkaState.Holders.Count > 0)
        {
            ____roundsLeft++;
            
            if (____runState.Players.Count > 1)
            {
                MatryoshkaState.IsActive = true;
            }
        }
    }
}

// Reminader of patches are for multiplayer functionality.
/*
[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), "AwardRelics")]
public static class AwardRelicsPatch
{
    public static void Postfix(
        TreasureRoomRelicSynchronizer __instance,
        IRunState ____runState,
        Rng ____rng,
        List<RelicModel>? ____currentRelics,
        ref int ____roundsLeft,
        List<TreasureRoomRelicSynchronizer.PlayerVote> ____votes)
    {
        if (____currentRelics == null || !MatryoshkaState.IsActive)
        {
            return;
        }

        return;
    }
}
*/


