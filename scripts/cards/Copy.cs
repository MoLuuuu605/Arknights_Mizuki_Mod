using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

using Arknights_Mizuki.Scripts.Pools;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Copy : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    // 记录当前复制的目标（用于打出时执行正确效果）
    private CardModel _currentCopyTarget;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        new RepeatVar(1)
    };
    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[1] {CardKeyword.Exhaust};

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/Copy.png";

    public Copy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        
        // 排除自己
        if (cardPlay.Card == this) return;
        
        var lastCard = cardPlay.Card;
        if (lastCard == null) return;
        
        // 记录当前复制的目标
        _currentCopyTarget = lastCard;
        
    }


    /// <summary>
    /// 打出时执行被复制的牌的效果
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_currentCopyTarget == null)
        {
            // 还没有复制任何牌，无事发生
            return;
        }
        
        // 创建一个临时克隆来执行效果
        for(int i = 0; i < this.DynamicVars.Repeat.BaseValue; i++)
        {
            var clonedCard = CombatState.CloneCard(_currentCopyTarget);
            
            if (clonedCard == null) return;
            CardCmd.ApplyKeyword(clonedCard,CardKeyword.Exhaust);
            // 复制升级状态
            if (_currentCopyTarget.IsUpgraded && clonedCard.IsUpgradable)
            {
                CardCmd.Upgrade(clonedCard);
            }
            
            // 复制动态变量
            foreach (var key in _currentCopyTarget.DynamicVars.Keys)
            {
                if (clonedCard.DynamicVars.ContainsKey(key))
                {
                    clonedCard.DynamicVars[key].BaseValue=_currentCopyTarget.DynamicVars[key].BaseValue;
                }
            }
            
            // 自动打出克隆牌
            await CardCmd.AutoPlay(choiceContext, clonedCard, null, AutoPlayType.Default);
        }
    }

    /// <summary>

    protected override void OnUpgrade()
    {
        this.DynamicVars.Repeat.UpgradeValueBy(1);
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}
