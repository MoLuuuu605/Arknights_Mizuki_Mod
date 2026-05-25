using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Shadow : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/Shadow.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new PowerVar<ShadowFlagPower>(1m)
    };

    public Shadow()
        : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, true)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay _)
    {
        await PowerCmd.Apply<ShadowFlagPower>(
            choiceContext,
            ((CardModel)this).Owner.Creature,
            ((CardModel)this).DynamicVars["ShadowFlagPower"].BaseValue,
            ((CardModel)this).Owner.Creature,
            (CardModel)(object)this,
            false
        );
    }

    protected override void OnUpgrade()
    {
        ((CardModel)this).AddKeyword(CardKeyword.Innate);
    }
}