using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class AberrantRegenerationPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/AberrantRegenerationPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/AberrantRegenerationPower.png";

    private bool drewThisTurn;

    public static async Task NotifyMaxHpLoss(PlayerChoiceContext choiceContext, Creature owner, decimal amount, CardPlay? cardPlay)
    {
        if (amount <= 0)
            return;

        await GoldLifeReturnPower.RecordMaxHpLoss(choiceContext, owner, amount, cardPlay?.Card);
        await PerfectAberrationPower.NotifyMaxHpLoss(choiceContext, owner, amount, cardPlay?.Card);

        AberrantRegenerationPower? power = owner.GetPower<AberrantRegenerationPower>();
        if (power == null)
            return;

        await CreatureCmd.GainBlock(owner, new BlockVar(amount, ValueProp.Move), cardPlay, false);

        if (power.Amount > 1 && !power.drewThisTurn)
        {
            power.drewThisTurn = true;
            await CardPileCmd.Draw(choiceContext, 1, owner.Player);
        }

        power.Flash();
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (Owner == player.Creature)
            drewThisTurn = false;

        return Task.CompletedTask;
    }
}
