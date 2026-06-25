using Arknights_Mizuki.Scripts.Actions;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Arknights_Mizuki.BaseLibAdapters;
using MinionLib.Minion;


namespace Arknights_Mizuki.Scripts.Minions;

public sealed class FloatingSeaMinion : CustomMinionModel
{
    public override int MinInitialHp => 5;
    public override int MaxInitialHp => 5;

    protected override string VisualsPath => CustomVisualPath;
    public override string CustomVisualPath => "res://Arknights_Mizuki/monsters/floater.tscn";
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    public FloatingSeaMinion()
    {
        RegisterSceneConversions();
    }

    public override async Task OnSummon(PlayerChoiceContext choiceContext,Player owner, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(this.Creature, maxHp);

        // 基础格挡数值（可通过其他方式增加）

        await PowerCmd.Apply<FloatingSeaBlockPower>(choiceContext,this.Creature,1,this.Creature,null);
        await PowerCmd.Apply<FloatingSeaBlockAction>(choiceContext,this.Creature,1,this.Creature,null);
    }
}
