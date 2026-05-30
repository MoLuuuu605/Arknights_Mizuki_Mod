using Arknights_Mizuki.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.Minion;

namespace Arknights_Mizuki.Scripts.Minions;

public sealed class FloatingSeaMinion : MinionModel
{
    public override int MinInitialHp => 12;
    public override int MaxInitialHp => 12;

    protected override string VisualsPath => "res://Arknights_Mizuki/monsters/floater.tscn";

    public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(self, maxHp);

        // 基础格挡数值（可通过其他方式增加）
        var choiceContext = new ThrowingPlayerChoiceContext();

        await PowerCmd.Apply<FloatingSeaBlockPower>(choiceContext,this.Creature,1,this.Creature,null);
    }
}