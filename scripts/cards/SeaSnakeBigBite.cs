using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.HoverTips;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;

namespace Arknights_Mizuki.Scripts.Cards;

// 注册卡牌。如果要写自定义池看添加人物的开头
[Pool(typeof(MzkCardPool))]
public class SeaSnakeBigBite : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Uncommon;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（例如这里是12点伤害）
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
	{
		(DynamicVar)new PowerVar<SanityPower>(5m)
	};
    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[2] { (CardKeyword)1,(CardKeyword)2 };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
	{
		HoverTipFactory.FromPower<SanityPower>()
	};

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/Sea_Snake_Bite.png";

    public SeaSnakeBigBite() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    // 打出时的效果逻辑
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SanityPower>(choiceContext, cardPlay.Target, ((DynamicVar)((CardModel)this).DynamicVars["SanityPower"]).BaseValue, ((CardModel)this).Owner.Creature, (CardModel)(object)this, false);
    }

    // 升级后的效果逻辑
    protected override void OnUpgrade()
    {
		((DynamicVar)((CardModel)this).DynamicVars["SanityPower"]).UpgradeValueBy(2m);
    }
}