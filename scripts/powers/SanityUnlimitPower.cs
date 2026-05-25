using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
/// 损伤伤害限制 每层增加损伤伤害上限20点
/// 叠加：依次为 25%(0层) → 40%(15层) → 55%(30层) → 65%(45层) ...
/// </summary>
public sealed class SanityUnlimitPower: CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
}