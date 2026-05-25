using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.keywords;
using MegaCrit.Sts2.Core.HoverTips;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Explain : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new EnergyVar(1)
    };

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/Explain.png";
    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[2] { CardKeyword.Exhaust, AutoPlay.Autoplay };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
	{
		HoverTipFactory.FromKeyword(AutoPlay.Autoplay)
	};
    public Explain() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
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
        await PlayerCmd.GainEnergy(((DynamicVar)((CardModel)this).DynamicVars["Energy"]).BaseValue, ((CardModel)this).Owner);
    }

    protected override void OnUpgrade()
    {
        ((CardModel)this).RemoveKeyword((CardKeyword)1);
    }
}