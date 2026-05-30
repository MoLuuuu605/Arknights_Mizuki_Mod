using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MinionLib.Minion;

public sealed class SacCrawlerMinion : MinionModel
{
    public override int MinInitialHp => 8;
    public override int MaxInitialHp => 8;
    protected override string VisualsPath => "res://Arknights_Mizuki/monsters/crawler.tscn";

    public override async Task OnSummon(Player owner, Creature self, MinionSummonOptions options)
    {
        if (options.MaxHp is decimal maxHp)
            await CreatureCmd.SetMaxAndCurrentHp(self, maxHp);

        decimal baseSanity = 2m;
        if (options.PrimaryStatAmount is decimal bonus && bonus > 0m)
            baseSanity += bonus;

        await PowerCmd.Apply<SacCrawlerSanityPower>(choiceContext:new ThrowingPlayerChoiceContext(),self, baseSanity, owner.Creature, options.Source);
    }
}