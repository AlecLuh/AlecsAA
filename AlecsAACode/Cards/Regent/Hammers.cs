using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace AlecsAA.AlecsAACode.Cards;

[Pool(typeof(RegentCardPool))]
public class Hammers() : AlecsAACard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<MinionReinforcement>(IsUpgraded)];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> selection = (await CardSelectCmd
            .FromCombatPile(choiceContext, PileType.Discard.GetPile(base.Owner), Owner, new CardSelectorPrefs(
                CardSelectorPrefs.TransformSelectionPrompt, DynamicVars.Cards.IntValue)))
            .ToList();

        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        
        foreach(CardModel item in selection)
        {
            CardPileAddResult? cardPileAddResult = await CardCmd.TransformTo<MinionReinforcement>(item);
            if (IsUpgraded && cardPileAddResult.HasValue)
            {
                CardCmd.Upgrade(cardPileAddResult.Value.cardAdded);
            }
        }
    }
}