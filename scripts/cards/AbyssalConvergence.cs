using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public sealed class AbyssalConvergence : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[3]
    {
        (DynamicVar)new DynamicVar("Float", 2m),
        (DynamicVar)new DynamicVar("Harvest", 2m),
        (DynamicVar)new DynamicVar("BonusRepeats", 0m)
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/AbyssalConvergence.png";

    public AbyssalConvergence() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int repeats = ResolveEnergyXValue() + DynamicVars["BonusRepeats"].IntValue;

        for (int i = 0; i < repeats; i++)
        {
            await GroupHatching.Float(choiceContext, (CardModel)(object)this, Owner, DynamicVars["Float"].BaseValue);
            await GroupHatching.Harvest(choiceContext, (CardModel)(object)this, Owner, DynamicVars["Harvest"].BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BonusRepeats"].UpgradeValueBy(1m);
    }
}
