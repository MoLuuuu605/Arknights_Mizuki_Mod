using Arknights_Mizuki.Scripts.Actions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
namespace Arknights_Mizuki.Scripts.Powers;


public sealed class FloatingSeaBlockPower : CustomPowerModel
{
    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/FloatingSeaBlockPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/FloatingSeaBlockPower.png";

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
            if (side != Owner.Side || !Owner.IsAlive) return;
            var applier = Owner.PetOwner?.Creature ?? Owner;
            await PowerCmd.Remove<FloatingSeaBlockAction>(Owner);
            await PowerCmd.Apply<FloatingSeaBlockAction>(choiceContext,Owner,1,Owner,null);
    }
}
