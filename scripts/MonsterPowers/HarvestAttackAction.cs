using Arknights_Mizuki.Scripts.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Arknights_Mizuki.BaseLibAdapters;

namespace Arknights_Mizuki.Scripts.Actions;

public sealed class HarvestAttackAction : CustomActionModel
{
    public override string CustomPackedIconPath => "res://Arknights_Mizuki/images/powers/HarvestAttackAction.png";
    public override string CustomBigIconPath => "res://Arknights_Mizuki/images/powers/HarvestAttackAction.png";

    public override TargetType TargetType => TargetType.AnyEnemy;
    public override bool AutoRemoveAtTurnEnd => true;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
    
    protected override async Task OnAct(PlayerChoiceContext choiceContext, Creature? target)
    {
        var baseDamage=4m;
        if (target == null) return;
        var actor = Owner;
        var enemies = CombatState.GetOpponentsOf(Owner).ToList(); // 需要确认实际API
        var extraDamage = actor.GetPowerAmount<SeabornizationPower>();
        baseDamage+=extraDamage;//TODO Change power
        for(int i=0;i<2;i++)
        {
            foreach (var enemy in enemies)
            {
                if (!enemy.IsDead)
                {
                    await CreatureCmd.Damage(choiceContext,enemy,baseDamage,ValueProp.Move,null);
                }
            }
        }
        await PowerCmd.Apply<HarvestAttackAction>(choiceContext,Owner,-1,Owner,null);
        await CreatureCmd.Damage(choiceContext,Owner,1,ValueProp.Unblockable|ValueProp.Unpowered,null,null);
    }

}
