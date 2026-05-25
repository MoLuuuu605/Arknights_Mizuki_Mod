using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Powers;

/// <summary>
/// 潜行：本回合受到的伤害减少30%（可叠层），每回合开始层数-1
/// </summary>
public sealed class ShadowPower : CustomPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/shadow.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/shadow.png";

    private const decimal DamageReductionPerStack = 0.3m;

    /// <summary>
    /// </summary>
    /// 
public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        await base.AfterSideTurnEnd(choiceContext, side, participants);
        
        // 只在轮到自己的回合结束时触发
        if (side != Owner.Side)
            return;
        
        // 层数 <= 0 时不造成伤害
        if (Amount <= 0)
            return;
        
        // 获取所有敌人
        var opponents = CombatState.GetOpponentsOf(Owner).ToList();
        
        // 对每个敌人造成等同于层数的伤害
        foreach (var enemy in opponents)
        {
            if (enemy != null && enemy.IsAlive)
            {
                await CreatureCmd.Damage(
                    choiceContext,
                    enemy,
                    Amount,
                    ValueProp.Move,  // 普通伤害，可被格挡
                    Owner,
                    null
                );
            }
        }
        await PowerCmd.Remove<ShadowPower>(Owner);
        Flash();
    }
}
