using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Commands.Builders;


namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
/// 攻击后给予目标一层 SanityPower
/// </summary>
public sealed class AttackApplySanityPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override bool AllowNegative => false;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/Sanity.png";

    private const int SanityAmount = 1;

    /// <summary>
    /// 攻击后触发
    /// </summary>
    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        await base.AfterAttack(choiceContext, command);
        
        if (command.Attacker != Owner) return;
        
        if (Amount <= 0) return;
        
        foreach (var hitResults in command.Results)
        {
            foreach (var damageResult in hitResults)
            {
                var target = damageResult.Receiver;
                
                if (target != null && target.IsAlive && target != Owner)
                {
                    // 施加的层数 = 当前能力层数
                    await PowerCmd.Apply<SanityPower>(
                        choiceContext,
                        target,
                        Amount,  // 使用当前层数
                        Owner,
                        null,
                        false
                    );
                }
            }
        }
        
        Flash();
    }
}