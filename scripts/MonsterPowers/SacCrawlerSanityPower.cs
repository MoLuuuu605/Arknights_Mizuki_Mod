using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class SacCrawlerSanityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/SacCrawlerSanityPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/SacCrawlerSanityPower.png";
}
