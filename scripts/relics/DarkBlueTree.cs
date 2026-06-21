using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;

namespace Arknights_Mizuki.Scripts.Relics;

/// <summary>
/// 深蓝之树：每场战斗你的第一次攻击会额外给予3层神经损伤(SanityPower),每次攻击额外附带1
/// </summary>
[Pool(typeof(MzkRelicPool))]
public class DarkBlueTree : CustomRelicModel
{

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[] { new PowerVar<SanityPower>(3m) };

    public override string PackedIconPath =>
        $"res://Arknights_Mizuki/images/relics/MzkTreeBranch.png";
    protected override string PackedIconOutlinePath =>
        $"res://Arknights_Mizuki/images/relics/MzkTreeBranch.png";
    protected override string BigIconPath =>
        $"res://Arknights_Mizuki/images/relics/MzkTreeBranch.png";

    public override async Task BeforeCombatStart()
    {
        var choiceContext=new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<AttackApplySanityPower>(
            choiceContext,
            Owner.Creature,
            2,
            Owner.Creature,
            null,
            false
        );
    }
    
}