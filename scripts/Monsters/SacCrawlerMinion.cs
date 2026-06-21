using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Arknights_Mizuki.BaseLibAdapters;
using MinionLib.Minion;


namespace Arknights_Mizuki.Scripts.Minions;
public sealed class SacCrawlerMinion : CustomMinionModel
{
    public override int MinInitialHp => 8;
    public override int MaxInitialHp => 8;
    protected override string VisualsPath => CustomVisualPath;
    public override string CustomVisualPath => "res://Arknights_Mizuki/monsters/crawler.tscn";
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    public SacCrawlerMinion()
    {
        RegisterSceneConversions();
    }

    public override async Task OnSummon(PlayerChoiceContext choiceContext,Player owner, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(this.Creature, maxHp);

        await PowerCmd.Apply<SacCrawlerSanityPower>(choiceContext,this.Creature, 1, owner.Creature, options.Source);
        await PowerCmd.Apply<SacCrawlerSanityAction>(choiceContext,this.Creature, 1, owner.Creature, options.Source);
    }
}
