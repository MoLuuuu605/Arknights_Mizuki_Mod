using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
///海嗣化，每若干层提高输出
/// </summary>
public sealed class SeabornizationPower: CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
}