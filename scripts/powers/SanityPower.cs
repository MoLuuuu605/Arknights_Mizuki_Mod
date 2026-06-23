using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

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
    private const int BaseDamagePercent = 20;
    private const int BaseDamageCap = 30;
    private const int DamageCapPerBurst = 15;
    private const int DamageCapPerUnlimit = 20;

    public void SetDamage(decimal damage)
	{
		AssertMutable();
		this.DynamicVars.Damage.BaseValue = damage;
	}
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromPower<SanityBurstDescriptionPower>()
    };
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(0m, ValueProp.Unpowered|ValueProp.Move)
    ];
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        decimal amount,
        Creature applier,
        CardModel cardSource)
    {
        if ((object)power != this)
            return;
        
        var triggerThreshold = TriggerThreshold;
        if (applier != null && applier.HasPower<SanityProBurstPower>())
        {
            triggerThreshold = 6;
        }

        var owner = Owner;
        if (owner == null || !owner.IsAlive)
            return;

        // 计算损伤伤害：最大生命值 * (BaseDamagePercent + SanityMultiplier)%
        var multiplier = owner.HasPower<SanityMultiplierPower>()
            ? owner.GetPower<SanityMultiplierPower>().Amount
            : 0;
        var damagePercent = BaseDamagePercent + multiplier;
        var maxHp = owner.MaxHp;
        var damage = maxHp * damagePercent / 100m;

        var Unlimit = applier != null && applier.HasPower<SanityBurstPower>()
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



        if (damage > damageCap)
            damage = damageCap;

        SetDamage(damage);

        if (Amount < triggerThreshold)
            return;
        // 造成 HpLoss 伤害

        
        await PowerCmd.Apply<PiercingWailPower>(
            choiceContext,
            owner, 
            1,
            owner, 
            cardSource,
            false
        );
        
        await CreatureCmd.Damage(
            choiceContext,
            owner,
            damage,
            ValueProp.Unpowered|ValueProp.Move,
            owner,
            cardSource);

        // 损伤层数 -8
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -triggerThreshold,
            null,
            null,
            false);

        if (applier != null)
            await PainEchoPower.Trigger(choiceContext, applier);

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
