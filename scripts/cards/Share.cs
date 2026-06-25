using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.keywords;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Share : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new DamageVar(8m, (ValueProp)6)
    };

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/share.png";
    public override IEnumerable<CardKeyword> CanonicalKeywords => [AutoPlay.Autoplay];

    public Share() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
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
        var opponents = ((CardModel)this).CombatState.GetOpponentsOf(Owner.Creature);
        var aliveOpponents = opponents.Where(o => o.IsAlive).ToList();
        if (aliveOpponents.Count > 0)
        {
            var randomTarget = aliveOpponents[new Random().Next(aliveOpponents.Count)];
            await DamageCmd.Attack(((DynamicVar)((CardModel)this).DynamicVars.Damage).BaseValue)
                .FromCard(this)
                .Targeting(randomTarget)
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        ((DynamicVar)((CardModel)this).DynamicVars.Damage).UpgradeValueBy(3m);
    }
}
