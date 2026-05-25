using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Arknights_Mizuki.Scripts.Pools;

public class MzkRelicPool : CustomRelicPoolModel
{
    // 描述中使用的能量图标。大小为24x24。
    public override string? TextEnergyIconPath => "res://Arknights_Mizuki/images/energy_mizuki.png";
    // tooltip和卡牌左上角的能量图标。大小为74x74。
    public override string? BigEnergyIconPath => "res://Arknights_Mizuki/images/energy_mizuki_big.png";
}