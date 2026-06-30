using Arknights_Mizuki.Scripts.Relics;
using BaseLib.Abstracts;
using BaseLib.Hooks;
using Godot;
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
public sealed class SanityPower : CustomPowerModel,IHealthBarForecastSource
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";

    private const int BaseDamage = 10;
    private const int TriggerThreshold = 8;
    private const int MultiplierIncrement = 10;
    private const int BaseDamagePercent = 20;
    private const int BaseDamageCap = 30;
    private const int DamageCapPerBurst = 15;
    private const int MultiplayerDamageCapPerBurst = 10;
    private const int DamageCapPerUnlimit = 20;
    private const int MultiplayerThresholdPerPlayer = 4;
    private const int InitialFormThresholdReduction = 2;

    public void SetDamage(decimal damage)
	{
		AssertMutable();
		this.DynamicVars.Damage.BaseValue = damage;
	}

    private void SetTriggerThreshold(decimal triggerThreshold)
    {
        AssertMutable();
        DynamicVars["TriggerThreshold"].BaseValue = triggerThreshold;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromPower<SanityBurstDescriptionPower>()
    };

    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
    {
        foreach (var segment in base.GetHealthBarForecastSegments(context))
            yield return segment;

        var owner = Owner;
        if (owner == null || context.Creature != owner || !owner.IsAlive)
            yield break;

        int damage = GetForecastDamage();
        if (damage <= 0)
            yield break;

        yield return new HealthBarForecastSegment(
            damage,
            new Color(0.4f, 0.8f, 1.0f, 0.95f),
            HealthBarForecastDirection.FromRight,
            20,
            null,
            new Color(0.4f, 0.8f, 1.0f, 0.95f)
        );
    }

    public int GetForecastDamage()
    {
        return (int)Math.Ceiling(DynamicVars.Damage.BaseValue);
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(0m, ValueProp.Unpowered|ValueProp.Move),
        new DynamicVar("TriggerThreshold", TriggerThreshold)
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
        
        var owner = Owner;
        if (owner == null || !owner.IsAlive)
            return;

        int playerCount = GetPlayerCount(owner);
        int triggerThreshold = GetTriggerThreshold(owner, playerCount);
        SetTriggerThreshold(triggerThreshold);
        var multiplier = owner.HasPower<SanityMultiplierPower>()
            ? owner.GetPower<SanityMultiplierPower>().Amount
            : 0;
        var damagePercent = BaseDamagePercent + multiplier;
        var maxHp = owner.MaxHp;
        var damage = maxHp * damagePercent / 100m;

        damage +=BaseDamage;

        var unlimit = applier != null && applier.HasPower<SanityBurstPower>()
            ? applier.GetPower<SanityBurstPower>().Amount
            : 0;

        if (unlimit != 0)
        {
            await PowerCmd.Apply<SanityUnlimitPower>(
            choiceContext,
            applier,
            unlimit,
            applier,
            cardSource,
            false);
        }

        // 计算爆发伤害上限
        // 基础上限25 + 已爆发次数*30 + 玩家SanityUnlimitPower层数*20
        var burstCount = multiplier / MultiplierIncrement;
        var unlimitAmount = GetSharedUnlimitAmount(owner);
        var damageCapPerBurst = DamageCapPerBurst + Math.Max(0, playerCount - 1) * MultiplayerDamageCapPerBurst;
        var damageCap = BaseDamageCap + burstCount * damageCapPerBurst + unlimitAmount * DamageCapPerUnlimit;



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
        await PowerCmd.Apply<WeakPower>(
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
            ValueProp.Unpowered|ValueProp.Unblockable,
            owner,
            cardSource);
        await PowerCmd.ModifyAmount(
            choiceContext,
            this,
            -triggerThreshold,
            null,
            null,
            false);

        if (applier != null)
            await PainEchoPower.Trigger(choiceContext, applier);

        PacmanCollectorsEdition? pacmanCollectorsEdition = applier?.Player?.GetRelic<PacmanCollectorsEdition>();
        if (pacmanCollectorsEdition != null)
        {
            pacmanCollectorsEdition.Flash();
            await PowerCmd.Apply<ShrinkPower>(
                choiceContext,
                owner,
                pacmanCollectorsEdition.DynamicVars["ShrinkPower"].BaseValue,
                applier,
                cardSource,
                false);
        }

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

    private static int GetPlayerCount(Creature owner)
    {
        return Math.Max(1, owner.CombatState?.Players.Count ?? 1);
    }

    private static int GetTriggerThreshold(Creature owner, int playerCount)
    {
        int initialFormCount = owner.CombatState?.Players.Count(player => player.Creature.HasPower<SanityProBurstPower>()) ?? 0;
        int threshold = TriggerThreshold
            + Math.Max(0, playerCount - 1) * MultiplayerThresholdPerPlayer
            - initialFormCount * InitialFormThresholdReduction;

        return Math.Max(1, threshold);
    }

    private static decimal GetSharedUnlimitAmount(Creature owner)
    {
        if (owner.CombatState == null)
            return 0;

        return owner.CombatState.Players
            .Where(player => player.Creature.HasPower<SanityUnlimitPower>())
            .Sum(player => player.Creature.GetPower<SanityUnlimitPower>().Amount);
    }
}
