using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MinionLib.Targeting;
using MinionLib.BaseLibAdapters;
using MegaCrit.Sts2.Core.Entities.Cards;
using Arknights_Mizuki.Scripts.Powers;

namespace Arknights_Mizuki.Scripts.Actions;

public sealed class FloatingSeaBlockAction : CustomActionModel
{
    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/FloatingSeaBlockAction.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/FloatingSeaBlockAction.png";

    public override TargetType TargetType => MinionTargetTypes.AnyMinionOrOwner;
    public override bool AutoRemoveAtTurnEnd => false;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
    {
        var baseBlock=5;
        if (target == null) return;
        var actor = Owner;
        var block = actor.GetPowerAmount<SeabornizationPower>();
        await CreatureCmd.GainBlock(target, block+baseBlock, ValueProp.Move, null);
        await PowerCmd.Remove<FloatingSeaBlockAction>(Owner);
    }
}
