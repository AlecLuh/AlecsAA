using AlecsAA.AlecsAACode.Relics;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace AlecsAA.AlecsAACode.Relics;

[Pool(typeof(EventRelicPool))]
public class Matryoshka() : AlecsAARelic
{
    private int _timesUsed;
    
    public override RelicRarity Rarity => RelicRarity.Event;
    
    public override bool IsUsedUp => TimesUsed >= DynamicVars["Chests"].IntValue;
    public override bool ShowCounter => TimesUsed < DynamicVars["Chests"].IntValue;
    
    public override int DisplayAmount => DynamicVars["Chests"].IntValue - TimesUsed;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Chests", 2m)];
    
    [SavedProperty]
    public int TimesUsed
    {
        get => _timesUsed;
        set
        {
            AssertMutable();
            _timesUsed = value;
            InvokeDisplayAmountChanged();
            CheckIfUsedUp();
        }
    }
    
    // Long-term, I will make this relic compatible with multiplayer.
    // Additional relics will appear in treasure chests for each person with this relic. If one person has it, the extra
    // will just go to them. If multiple people do, they will have a second round to vote on what they want.
    protected override bool IsAllowedInternal(IRunState runState)
    {
        return runState.Players.Count == 1;
    }
    
    private void CheckIfUsedUp()
    {
        if (IsUsedUp)
        {
            Status = RelicStatus.Disabled;
        }
    }
}