using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class BlueLifeBloomPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override bool ShouldReceiveCombatHooks => true;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/BlueLifeBloomPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/BlueLifeBloomPower.png";

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Amount <= 0)
            return;

        await CreatureCmd.GainMaxHp(Owner, Amount);
        Flash();
    }
}
