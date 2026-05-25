using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
/// 损伤：达到8层时，目标受到最大生命值*25%的伤害（有上限），层数-8并增加损伤倍率
/// </summary>
public sealed class SanityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";

    private const int TriggerThreshold = 8;
    private const int MultiplierIncrement = 15;
    private const int BaseDamagePercent = 25;
    private const int BaseDamageCap = 25;
    private const int DamageCapPerBurst = 30;
    private const int DamageCapPerUnlimit = 20;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature applier,
        CardModel cardSource)
    {
        // 只处理自身的变化，且层数必须 >= 阈值
        if ((object)power != this || Amount < TriggerThreshold)
            return;

        var owner = Owner;
        if (owner == null || !owner.IsAlive)
            return;

        await PowerCmd.Apply<PiercingWailPower>(
            choiceContext,
            owner, 
            1,
            owner, 
            cardSource,
            false
        );
        // 计算损伤伤害：最大生命值 * (BaseDamagePercent + SanityMultiplier)%
        var multiplier = owner.HasPower<SanityMultiplierPower>()
            ? owner.GetPower<SanityMultiplierPower>().Amount
            : 0;
        var damagePercent = BaseDamagePercent + multiplier;
        var maxHp = owner.MaxHp;
        var damage = maxHp * damagePercent / 100m;

        var Unlimit = applier.HasPower<SanityBurstPower>()
            ? applier.GetPower<SanityBurstPower>().Amount
            : 0;

        if(Unlimit != 0 ){
            await PowerCmd.Apply<SanityUnlimitPower>(
            choiceContext,
            applier,
            Unlimit,
            applier,
            cardSource,
            false);
        }

        // 计算爆发伤害上限
        // 基础上限25 + 已爆发次数*30 + 玩家SanityUnlimitPower层数*20
        var burstCount = multiplier / MultiplierIncrement;
        var unlimitAmount = applier != null && applier.HasPower<SanityUnlimitPower>()
            ? applier.GetPower<SanityUnlimitPower>().Amount
            : 0;
        var damageCap = BaseDamageCap + burstCount * DamageCapPerBurst + unlimitAmount * DamageCapPerUnlimit;

        // 伤害不超过上限
        if (damage > damageCap)
            damage = damageCap;

        // 造成 HpLoss 伤害
        await CreatureCmd.Damage(
            choiceContext,
            owner,
            damage,
            ValueProp.Unblockable | ValueProp.Unpowered,
            owner,
            cardSource);

        // 损伤层数 -8
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -TriggerThreshold,
            null,
            null,
            false);

        // 增加损伤倍率
        await PowerCmd.Apply<SanityMultiplierPower>(
            choiceContext,
            owner,
            MultiplierIncrement,
            owner,
            cardSource,
            false);

        Flash();
    }
}