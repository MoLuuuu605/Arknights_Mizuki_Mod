using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Powers;

public class SeaCreature: CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override PowerInstanceType InstanceType => PowerInstanceType.None;

	public override string? CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/SeaCreatureForm.png";

	public override string? CustomBigIconPath => "res://Arknights_Mizuki/images/powers/SeaCreatureForm.png";
	protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        new DamageVar(4m,ValueProp.Unpowered),
        new PowerVar<AttackApplySanityPower>(2)
	};
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var choiceContext=new ThrowingPlayerChoiceContext();
        bool Change = Owner.HasPower<Human>();
        if(Change)
        {
            await PowerCmd.Remove<Human>(Owner);
            if(Owner.HasPower<StealthPower>())
            {
                await PowerCmd.Apply<StealthPower>(choiceContext,Owner,-1,Owner,null);
            }
        }
        await CreatureCmd.Damage(choiceContext,Owner, DynamicVars.Damage.BaseValue,ValueProp.Unblockable | ValueProp.Unpowered,Owner,null);
        await PowerCmd.Apply<AttackApplySanityPower>(choiceContext,Owner,DynamicVars["AttackApplySanityPower"].BaseValue,Owner,null);
    }
    public override decimal ModifyDamageMultiplicative(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == Owner)
            return 1.25m;

        if (!props.IsPoweredAttack())
            return 1m;

        decimal multiplier = 1.5m;
        if (multiplier < 0m)
            multiplier = 0m;

        return multiplier;
    }
}
