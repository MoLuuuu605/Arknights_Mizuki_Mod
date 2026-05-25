using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

using Arknights_Mizuki.Scripts.Pools;
using MegaCrit.Sts2.Core.HoverTips;

namespace Arknights_Mizuki.Scripts.Cards;

// 注册卡牌到 MzkCardPool
[Pool(typeof(MzkCardPool))]
public class Overload : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 0;
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
public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[1] { CardKeyword.Exhaust };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[2]
	{
		HoverTipFactory.FromCard<Share>(),
        HoverTipFactory.FromCard<Hurt>()
	};
    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/Overload.png";
    public Overload() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Add(CombatState.CreateCard<Share>(Owner), PileType.Draw,CardPilePosition.Random,null,false);
        await CardPileCmd.Add(CombatState.CreateCard<Hurt>(Owner), PileType.Discard,CardPilePosition.Random,null,false);
        await CardPileCmd.Add(CombatState.CreateCard<Share>(Owner), PileType.Draw,CardPilePosition.Bottom,null,false);
        await CardPileCmd.Add(CombatState.CreateCard<Share>(Owner), PileType.Draw,CardPilePosition.Bottom,null,false);
        await CardPileCmd.Draw(choiceContext, 1, ((CardModel)this).Owner, false);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}