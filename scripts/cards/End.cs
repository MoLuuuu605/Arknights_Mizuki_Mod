using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.HoverTips;

using Arknights_Mizuki.Scripts.Pools;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class End : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;
    public override int MaxUpgradeLevel => 999;
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        (DynamicVar)new DamageVar(3m, (ValueProp)8),
        (DynamicVar)new RepeatVar(3)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[0];

    // public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/End.png";

    public End() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成2点伤害4
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(this.DynamicVars.Repeat.IntValue)
            .FromCard(this)
            .TargetingRandomOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}