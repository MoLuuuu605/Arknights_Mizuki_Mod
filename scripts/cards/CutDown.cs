using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

using Arknights_Mizuki.Scripts.Pools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class CutDown : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;  // 蓝色
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        (DynamicVar)new DamageVar(8m,ValueProp.Move),      // 伤害 8 → 11
        (DynamicVar)new PowerVar<PiercingWailPower>(3)  // 减力量 3（不升级）
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/Cutdown.png";

    public CutDown() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        if (target == null)
            return;
        
        // 造成伤害
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(target)
            .Execute(choiceContext);
        
        // 施加临时减力量（PiercingWailPower）
        await PowerCmd.Apply<PiercingWailPower>(
            choiceContext,
            target,
            base.DynamicVars["PiercingWailPower"].BaseValue,
            base.Owner.Creature,
            this,
            false
        );
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}