using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Powers;

public class Human: CustomPowerModel
{
	public override PowerType Type => PowerType.Buff;

	public override PowerStackType StackType => PowerStackType.Single;

	public override PowerInstanceType InstanceType => PowerInstanceType.None;

	public override string? CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Human.png";

	public override string? CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Human.png";

	protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[4]
    {
		(DynamicVar)new EnergyVar(2),
        new CardsVar(3),
        new HealVar(6m),
        new PowerVar<StealthPower>(1m)
	};
    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        var choiceContext=new ThrowingPlayerChoiceContext();
        bool Change = Owner.HasPower<SeaCreature>();
        if(Change)
        {
            await PowerCmd.Remove<SeaCreature>(Owner);
            await PowerCmd.Apply<AttackApplySanityPower>(choiceContext,Owner,-2,Owner,null);
        }
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, ((PowerModel)this).Owner.Player);
        await CreatureCmd.Heal(Owner,DynamicVars.Heal.BaseValue);
        await CardPileCmd.Draw(choiceContext,DynamicVars.Cards.BaseValue,Owner.Player);
        await PowerCmd.Apply<StealthPower>(choiceContext,Owner,1,Owner,null);
    }
}
