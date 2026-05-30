using Arknights_Mizuki.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Powers;
using MinionLib.BaseLibAdapters;
public sealed class SacCrawlerSanityAction : CustomActionModel
{
    public override TargetType TargetType => TargetType.AnyEnemy; // 选择一个敌人
    public override bool AutoRemoveAtTurnEnd => true;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
    {
        if (target == null) return;
        var actor = Owner;
        var baseSanity=2;
        var extraSanity = Owner.GetPowerAmount<SeabornizationPower>();

        var finalSanity = baseSanity + extraSanity / 3;
        await PowerCmd.Apply<SanityPower>(choiceContext,target, finalSanity, actor, null);
        await PowerCmd.Remove<SacCrawlerSanityAction>(Owner);
    }
}