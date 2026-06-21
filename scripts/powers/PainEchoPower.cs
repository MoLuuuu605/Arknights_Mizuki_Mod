using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Arknights_Mizuki.Scripts.Powers;

public sealed class PainEchoPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/PainEchoPower.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/PainEchoPower.png";

    public static async Task Trigger(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Creatures.Creature owner)
    {
        int amount = owner.GetPowerAmount<PainEchoPower>();
        if (amount <= 0)
            return;

        await CardPileCmd.Draw(choiceContext, amount, owner.Player);
    }
}
