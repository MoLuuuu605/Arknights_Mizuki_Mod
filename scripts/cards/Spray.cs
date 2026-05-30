using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.keywords;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Spray : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        (DynamicVar)new DamageVar(3m, (ValueProp)5),
        (DynamicVar)new PowerVar<VulnerablePower>(1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromPower<VulnerablePower>()
    };

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/Spray.png";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [AutoPlay.Autoplay];

    public Spray() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await CardCmd.AutoPlay(choiceContext, card, null, AutoPlayType.Default);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(((CardModel)this).CombatState)
            .Execute(choiceContext);
        var opponents = ((CardModel)this).CombatState.GetOpponentsOf(Owner.Creature);
        foreach (var opponent in opponents)
        {
            await PowerCmd.Apply<VulnerablePower>(
                choiceContext,
                opponent,
                ((DynamicVar)((CardModel)this).DynamicVars["VulnerablePower"]).BaseValue,
                ((CardModel)this).Owner.Creature,
                (CardModel)(object)this,
                false
            );
        }
    }

    protected override void OnUpgrade()
    {
        ((CardModel)this).DynamicVars.Damage.UpgradeValueBy(2);
        this.DynamicVars["VulnerablePower"].UpgradeValueBy(1);
    }
}
