using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(TokenCardPool))]
public class BabyHs : CustomCardModel
{
    private const int energyCost = 0;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        (DynamicVar)new DamageVar(4m, (ValueProp)8),
        (DynamicVar)new CardsVar(1)
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/BabyHs.png";

    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[1]
    {
        CardKeyword.Exhaust
    };

    public BabyHs() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);

        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
