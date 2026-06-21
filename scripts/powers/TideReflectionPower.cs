using Arknights_Mizuki.Scripts.keywords;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class TideReflectionPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/TideReflectionPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/TideReflectionPower.png";

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;

        if (!cardPlay.Card.Keywords.Contains(AutoPlay.Autoplay))
            return;

        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, cardPlay);

        var enemies = CombatState.GetOpponentsOf(Owner)
            .Where(enemy => enemy.IsAlive)
            .ToList();
        if (enemies.Count > 0)
        {
            var target = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(enemies);
            await PowerCmd.Apply<SanityPower>(choiceContext, target, Amount, Owner, cardPlay.Card, false);
        }

        Flash();
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (Owner == player.Creature)
            await PowerCmd.Remove((PowerModel)(object)this);
    }
}
