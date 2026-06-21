using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class GoldLifeReturnPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;
    public override bool ShouldReceiveCombatHooks => true;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/GoldLifeReturnPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/GoldLifeReturnPower.png";

    public static async Task RecordMaxHpLoss(PlayerChoiceContext choiceContext, Creature owner, decimal amount, CardModel? source)
    {
        if (amount <= 0)
            return;

        GoldLifeReturnPower? power = owner.GetPower<GoldLifeReturnPower>();
        if (power == null)
            return;

        await PowerCmd.ModifyAmount(choiceContext, power, amount, owner, source, false);
        power.Flash();
    }

    public static decimal GetTrackedMaxHpLoss(Creature owner)
    {
        return owner.GetPower<GoldLifeReturnPower>()?.Amount ?? 0m;
    }

    public override Task AfterApplied(Creature applier, CardModel cardSource)
    {
        if (Amount > 0)
            SetAmount(Amount - 1, false);

        return Task.CompletedTask;
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (Amount <= 0)
            return;

        await CreatureCmd.GainMaxHp(Owner, Amount);
        SetAmount(0, false);
    }
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(Amount < 0)
        SetAmount(0,false);
    }

}
