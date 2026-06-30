using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class ExplainPower : CustomPowerModel
{
    public const int CardPerEnergy = 6;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/ExplainPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/ExplainPower.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int remaining = await PowerCmd.ModifyAmount(choiceContext, this, -1, Owner, cardPlay.Card, false);
        if (remaining > 0)
            return;
        var playernum=Owner.CombatState.Players.Count();
        var usingAmount = playernum * CardPerEnergy;
        foreach (var player in Owner.CombatState.Players.Where(player => player.Creature.IsAlive))
        {
            await PlayerCmd.LoseEnergy(1, player);
        }

        Flash();
        await PowerCmd.Apply<ExplainPower>(choiceContext, Owner, usingAmount, Owner, null, false);
    }
}
