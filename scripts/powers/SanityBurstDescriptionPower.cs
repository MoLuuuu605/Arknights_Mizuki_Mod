using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
/// 损伤爆发，当爆发损伤时获得一层反移情;
/// </summary>
public sealed class SanityBurstDescriptionPower: CustomPowerModel
{   
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("DamageUpgrade",15)
        ];

    public override async  Task BeforeCombatStart()
    {
        int playercount=Math.Max(1, CombatState?.Players.Count ?? 1);
        this.DynamicVars["DamageUpgrade"].BaseValue = 5 + playercount * 10;
    }

        private static int GetPlayerCount(Creature owner)
    {
        return Math.Max(1, owner.CombatState?.Players.Count ?? 1);
    }

}