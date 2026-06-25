using Arknights_Mizuki.Scripts.Actions;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MinionLib.Minion;
using Arknights_Mizuki.BaseLibAdapters;

namespace Arknights_Mizuki.Scripts.Minions;
public sealed class HarvestMinion : CustomMinionModel
{
    public override int MinInitialHp => 8;
    public override int MaxInitialHp => 8;
    protected override string VisualsPath => CustomVisualPath;
    public override string CustomVisualPath => "res://Arknights_Mizuki/monsters/harvest.tscn";
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);
    public int baseDamage = 3;

    public HarvestMinion()
    {
        RegisterSceneConversions();
    }

    public override async Task OnSummon(PlayerChoiceContext ChoiceContext,Player owner, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(this.Creature, maxHp);
        var choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<HarvestAttackPower>(choiceContext,this.Creature,1,this.Creature,null);
        await PowerCmd.Apply<HarvestAttackAction>(choiceContext,this.Creature,1,this.Creature,null);
    }


}
