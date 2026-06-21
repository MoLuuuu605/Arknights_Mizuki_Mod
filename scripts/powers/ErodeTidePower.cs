using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class ErodeTidePower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/erode_tide.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/erode_tide.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        if (cardPlay.Card.Owner != Owner.Player)
            return;
        
        if (Amount <= 0)
            return;

        var opponents = CombatState.GetOpponentsOf(Owner).ToList();
        foreach (var enemy in opponents)
        {
            if (enemy.IsAlive)
            {
                await PowerCmd.Apply<SanityPower>(
                    choiceContext,
                    enemy,
                    Amount,
                    Owner,
                    cardPlay.Card,
                    false);
            }
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner != player.Creature)
            return;

        await PowerCmd.Remove((PowerModel)(object)this);
    }
}
