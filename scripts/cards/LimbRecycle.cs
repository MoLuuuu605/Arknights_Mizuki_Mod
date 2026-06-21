using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public sealed class LimbRecycle : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[3]
    {
        (DynamicVar)new DamageVar(6m, (ValueProp)8),
        (DynamicVar)new DynamicVar("MaxHpGain", 10m),
        (DynamicVar)new PowerVar<SanityPower>(1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromPower<SanityPower>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1]
    {
        CardKeyword.Exhaust
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/LimbRecycle.png";

    public LimbRecycle() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(((CardModel)this).CombatState)
            .Execute(choiceContext);

        if (DynamicVars["SanityPower"].BaseValue > 0)
        {
            var opponents = ((CardModel)this).CombatState
                .GetOpponentsOf(Owner.Creature)
                .Where(opponent => opponent.IsAlive)
                .ToList();

            foreach (var opponent in opponents)
            {
                await PowerCmd.Apply<SanityPower>(
                    choiceContext,
                    opponent,
                    DynamicVars["SanityPower"].BaseValue,
                    Owner.Creature,
                    (CardModel)(object)this,
                    false);
            }
        }
        if(Owner.HasPower<GoldLifeReturnPower>())
        {
            await PowerCmd.Apply<GoldLifeReturnPower>(choiceContext,Owner.Creature,-8m,Owner.Creature,this);
        }
        decimal maxHpGain = DynamicVars["MaxHpGain"].BaseValue;
        await CreatureCmd.GainMaxHp(Owner.Creature, maxHpGain);
        await PerfectAberrationPower.NotifyMaxHpGain(choiceContext, Owner.Creature, maxHpGain, (CardModel)(object)this);
        
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
