using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public sealed class HiveWill : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new PowerVar<HiveWillPower>(2m),
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[2]
    {
        HoverTipFactory.FromPower<HiveWillPower>(),
        HoverTipFactory.FromPower<SeabornizationPower>()
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/HiveWill.png";

    public HiveWill() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal amount = DynamicVars["HiveWillPower"].BaseValue;
        await HiveWillPower.ApplyToMinions(choiceContext, Owner, amount);
        await PowerCmd.Apply<HiveWillPower>(choiceContext, Owner.Creature, amount, Owner.Creature, (CardModel)(object)this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HiveWillPower"].UpgradeValueBy(1m);
    }
}
