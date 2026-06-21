using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Relics;

/// <summary>
/// 深蓝之树的树枝：每场战斗你的第一次攻击会额外给予3层神经损伤(SanityPower)
/// </summary>
[Pool(typeof(MzkRelicPool))]
public class MzkTreeBranch : CustomRelicModel
{

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<SanityPower>(5m) };

    public override string PackedIconPath =>
        $"res://Arknights_Mizuki/images/relics/DarkBlueSeed.png";
    protected override string PackedIconOutlinePath =>
        $"res://Arknights_Mizuki/images/relics/DarkBlueSeed.png";
    protected override string BigIconPath =>
        $"res://Arknights_Mizuki/images/relics/DarkBlueSeed.png";

    public override async Task BeforeCombatStart()
    {
        await PowerCmd.Apply<SanityBuffPower>(
            new ThrowingPlayerChoiceContext(), 
            Owner.Creature, 
            (int)DynamicVars["SanityPower"].BaseValue, 
            Owner.Creature, 
            null, 
            false
        );
    }
    	public override RelicModel GetUpgradeReplacement()
	{
		return (RelicModel)(object)ModelDb.Relic<DarkBlueTree>();
	}
}