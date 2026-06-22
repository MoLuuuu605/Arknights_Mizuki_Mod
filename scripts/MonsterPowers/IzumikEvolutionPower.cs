using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class IzumikEvolutionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/IzumikEvolutionPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/IzumikEvolutionPower.png";
}
