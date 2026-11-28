using AlecsAA.AlecsAACode.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.TreasureRoomRelic;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;

namespace AlecsAA.AlecsAACode.Other.Patches;

internal static class MatryoshkaInfo
{
    public static readonly List<Player> Holders = new();
    public static readonly int MaxRelics = 8;

    public static bool IsHoldersOnlyRound(int currentRound, int rounds)
        => Holders.Count > 0 && currentRound == rounds - 1;

}

// If chest isn't empty, increments the number of relics in the chest for each player with active Matryoshka, and
// increases the number of rounds by 1 if anyone has an active Matryoshka. 
[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), nameof(TreasureRoomRelicSynchronizer.BeginRelicPicking))]
public static class BeginRelicPickingPatch
{
    static void Postfix(
        IRunState ____runState,
        RelicGrabBag ____sharedGrabBag,
        Rng ____rng,
        List<RelicModel>? ____currentRelics,
        ref int ____rounds)
    {
        MatryoshkaInfo.Holders.Clear();

        if (____currentRelics == null)
        {
            return;
        }
        
        foreach (Player player in ____runState.Players)
        {
            if (____currentRelics.Count >= MatryoshkaInfo.MaxRelics)
            {
                break;
            }
            
            Matryoshka? matryoshka = player.Relics.OfType<Matryoshka>().FirstOrDefault(r => !r.IsUsedUp);
            if (matryoshka != null)
            {
                RelicRarity rarity = RelicFactory.RollRarity(____rng);
                RelicModel item = ____sharedGrabBag.PullFromFront(rarity, ____runState) ?? RelicFactory.FallbackRelic;
                MatryoshkaInfo.Holders.Add(player);
                ____currentRelics.Add(item);

                matryoshka.TimesUsed++;
            }
        }

        if (MatryoshkaInfo.Holders.Count > 0)
        {
            ____rounds++;
        }
    }
}

// Prevents voting for people without Matryoshka during holders-only voting round.
[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), "FlushBufferedVotes")]
public static class HoldersOnlyRoundVoteSkipPatch
{
    static void Prefix(
        IRunState ____runState,
        List<TreasureRoomRelicSynchronizer.PlayerVote> ____votes,
        int ____currentRound,
        int ____rounds)
    {
        // Check if there are people with Matryoshka and if it's the final round (which is the Holders-only round).
        if (MatryoshkaInfo.IsHoldersOnlyRound(____currentRound, ____rounds))
        {
            for(int i = 0; i < ____runState.Players.Count; i++)
            {
                // If a player doesn't have Matryoshka, force a vote to skip relic-picking in the final round.
                if(!MatryoshkaInfo.Holders.Contains(____runState.Players[i]))
                {
                    ____votes[i].index = null;
                    ____votes[i].voteReceived = true;
                }
            }
        }
    }
}

// Prevents recasting vote for people without Matryoshka during holders-only voting round.
[HarmonyPatch(typeof(TreasureRoomRelicSynchronizer), nameof(TreasureRoomRelicSynchronizer.PickRelicLocally))]
public static class PickRelicLocallyPatch
{
    static bool Prefix(int ____currentRound, int ____rounds)
    {
        if (MatryoshkaInfo.IsHoldersOnlyRound(____currentRound, ____rounds))
        {
            // During holders-only round, only lets PickRelicLocally run if player is a holder.
            return MatryoshkaInfo.Holders.Any(player => LocalContext.IsMe(player));
        }

        return true;
    }
}

// Prevents OutOfRange exception caused by player slot being greater than number of relic holder UI nodes.
// Works by clamping player slots where needed.
[HarmonyPatch(typeof(NTreasureRoomRelicCollection), nameof(NTreasureRoomRelicCollection.DefaultFocusedControl), MethodType.Getter)]
public static class FocusGetterFix
{
    static bool Prefix(
        ref Control? __result,
        List<NTreasureRoomRelicHolder> ____holdersInUse,
        IRunState ____runState)
    {
        if (____holdersInUse.Count == 0)
        {
            return true;
        }

        int slot = ____runState.GetPlayerSlotIndex(LocalContext.GetMe(____runState.Players));

        if (slot >= 0 && slot < ____holdersInUse.Count)
        {
            return true;
        }

        __result = ____holdersInUse[Math.Clamp(slot, 0, ____holdersInUse.Count - 1)];
        return false;
    }
}

// Hides hands for non-holders during holders-only round and shows their cursor instead.
[HarmonyPatch(typeof(NHandImageCollection), nameof(NHandImageCollection.AfterNewRelicPickingRoundStarted))]
public static class HideNonHolderHandsPatch
{
    static void Postfix(List<NHandImage> ____hands)
    {
        var sync = Traverse.Create(RunManager.Instance.TreasureRoomRelicSynchronizer);
        bool holdersOnly = MatryoshkaInfo.IsHoldersOnlyRound(
            sync.Field("_currentRound").GetValue<int>(),
            sync.Field("_rounds").GetValue<int>());
        
        if (!holdersOnly)
        {
            return;
        }

        foreach (NHandImage hand in ____hands.Where(hand => !MatryoshkaInfo.Holders.Contains(hand.Player)))
        {
            // Hides hand of non-holders.
            hand.Visible = false;
            
            // Shows cursor on respective non-holder client.
            if (LocalContext.IsMe(hand.Player))
            {
                NGame.Instance.CursorManager.SetCursorShown(true);
                AccessTools.Method(typeof(NCursorManager), "UpdateCursor")
                    .Invoke(NGame.Instance.CursorManager, null);
            }
        }
    }
}

//  Flags whether cursor should stay active.
[HarmonyPatch(typeof(NHandImageCollection), "UpdateHandVisibility")]
public static class ShowCursorFlag
{
    public static bool ShouldShowCursor;

    static void Prefix(NHandImageCollection __instance)
    {
        NHandImage? localHand = __instance.GetHand(LocalContext.NetId.Value);
        ShouldShowCursor = localHand != null && !localHand.Visible;
    }

    // Resets flag.
    static void Finalizer() => ShouldShowCursor = false;
}

// Keeps cursor on if flag is true.
[HarmonyPatch(typeof(NCursorManager), "RefreshCursorShown")]
public static class ShowCursorPatch
{
    static void Prefix(ref bool ____shouldShowCursor)
    {
        if (ShowCursorFlag.ShouldShowCursor)
        {
            ____shouldShowCursor = true;
        }
    }
}


