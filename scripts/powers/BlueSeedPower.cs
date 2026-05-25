using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Powers;

public class BlueSeedPower : CustomPowerModel
{
	public override PowerType Type => (PowerType)1;

	public override PowerStackType StackType => (PowerStackType)2;

	public override PowerInstanceType InstanceType => (PowerInstanceType)1;

	public override string? CustomPackedIconPath => "res://images/powers/blue_seed.png";

	public override string? CustomBigIconPath => "res://images/powers/blue_seed.png";

	protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
		(DynamicVar)new EnergyVar(1),
	};	
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
            await PlayerCmd.GainEnergy(1m, ((PowerModel)this).Owner.Player);
    }
}

