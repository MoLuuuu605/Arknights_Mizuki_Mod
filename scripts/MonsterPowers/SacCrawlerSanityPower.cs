using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

public sealed class SacCrawlerSanityPower : CustomPowerModel
{
    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/SacCrawlerSanityPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/SacCrawlerSanityPower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || !Owner.IsAlive) return;
        var applier = Owner.PetOwner?.Creature ?? Owner;

        decimal baseAmount = Amount;
        int seabornStacks = Owner.GetPowerAmount<SeabornizationPower>();
        decimal bonus = seabornStacks / 3m;
        decimal finalAmount = baseAmount + bonus;
        await PowerCmd.Remove<SacCrawlerSanityAction>(Owner);
        await PowerCmd.Apply<SacCrawlerSanityAction>(choiceContext,Owner, finalAmount, applier, null);
    }
}
