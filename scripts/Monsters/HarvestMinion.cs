using Arknights_Mizuki.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.BaseLibAdapters;
using MinionLib.Minion;

public sealed class HarvestMinion : CustomMinionModel
{
    public override int MinInitialHp => 10;
    public override int MaxInitialHp => 10;
    protected override string VisualsPath => "res://Arknights_Mizuki/monsters/harvest.tscn";
    public int baseDamage = 3;
    public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(self, maxHp);
        var choiceContext = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<HarvestAttackPower>(choiceContext,this.Creature,1,this.Creature,null);
    }


}
