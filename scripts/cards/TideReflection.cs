using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public sealed class TideReflection : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[3]
    {
        (DynamicVar)new PowerVar<TideReflectionPower>(1m),
        (DynamicVar)new PowerVar<SanityPower>(1m),
        (DynamicVar)new CardsVar(2)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromPower<TideReflectionPower>()
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/TideReflection.png";

    public TideReflection() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TideReflectionPower>(choiceContext, Owner.Creature, DynamicVars["TideReflectionPower"].BaseValue, Owner.Creature, (CardModel)(object)this);
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CreateRandomAutoCard(), PileType.Discard, Owner, CardPilePosition.Random));
        }
    }

    private CardModel CreateRandomAutoCard()
    {
        int[] autoCards =
        {
            0,
            1,
            2,
            3,
            4,
            5
        };
        return Owner.RunState.Rng.CombatCardSelection.NextItem(autoCards) switch
        {
            0 => CombatState.CreateCard<Learn>(Owner),
            1 => CombatState.CreateCard<Share>(Owner),
            2 => CombatState.CreateCard<Spray>(Owner),
            3 => CombatState.CreateCard<Shock>(Owner),
            4 => CombatState.CreateCard<WaterSheild>(Owner),
            _ => CombatState.CreateCard<Explain>(Owner)
        };
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
