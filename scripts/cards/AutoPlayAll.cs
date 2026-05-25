using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using Arknights_Mizuki.Scripts.Pools;
using MegaCrit.Sts2.Core.HoverTips;
using Arknights_Mizuki.Scripts.keywords;

namespace Arknights_Mizuki.Scripts.Cards;

// 注册卡牌到 MzkCardPool
[Pool(typeof(MzkCardPool))]
public class AutoPlayAll : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型（防御牌是技能类型）
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（Self表示自己）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（格挡值）
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[0];
    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[2] {CardKeyword.Exhaust,AutoPlay.Autoplay};
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[3]
	{
		HoverTipFactory.FromCard<Learn>(),
        HoverTipFactory.FromCard<Share>(),
        HoverTipFactory.FromCard<Explain>()
	};
    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/AutoPlayAll.png";
    public AutoPlayAll() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        await CardPileCmd.Add(CombatState.CreateCard<Learn>(Owner), PileType.Discard,CardPilePosition.Random,null,false);
        await CardPileCmd.Add(CombatState.CreateCard<Share>(Owner), PileType.Discard,CardPilePosition.Random,null,false);
        await CardPileCmd.Add(CombatState.CreateCard<Explain>(Owner), PileType.Discard,CardPilePosition.Random,null,false);
        await CardPileCmd.Add(CombatState.CloneCard(this), PileType.Hand,CardPilePosition.Bottom,null,false);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}