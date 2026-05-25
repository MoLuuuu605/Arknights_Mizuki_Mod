using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class Bigge : CustomCardModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;

    // 参考官方写法：使用 CardsVar 或者 IntVar
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new IntVar("DiscardPicks", 1m)  // 基础 1，升级后 3
    };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/bigge.png";

    public Bigge() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary, true)
    {
    }
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取最多可消耗数量
        int maxPicks = base.DynamicVars["DiscardPicks"].IntValue;
        
        if (maxPicks < 1)
            return;
        
        // 获取弃牌堆
        CardPile discardPile = PileType.Discard.GetPile(base.Owner);
        
        if (discardPile.Cards.Count == 0)
            return;
        
        // 实际可消耗数量
        int pickCount = System.Math.Min(maxPicks, discardPile.Cards.Count);
        
        // 🔧 修改点：使用 FromSimpleGrid 替代 FromCombatPile
        List<CardModel> pickedCards = (await CardSelectCmd.FromSimpleGrid(
            choiceContext,
            discardPile.Cards,  // 注意：这里传 .Cards，不是整个 pile
            base.Owner,
            new CardSelectorPrefs(this.SelectionScreenPrompt,pickCount)  // 提示文本可以为空
        )).ToList();
        
        // 如果没有选择任何牌，不触发效果
        if (pickedCards.Count == 0)
            return;
        
        // 消耗选中的牌
        foreach (CardModel card in pickedCards)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
        
        int consumedCount = pickedCards.Count;
        
        // 对所有敌人施加等同于消耗牌数量的 SanityPower
        var opponents = ((CardModel)this).CombatState.GetOpponentsOf(Owner.Creature).ToList();
        foreach (var enemy in opponents)
        {
            if (enemy != null && enemy.IsAlive)
            {
                await PowerCmd.Apply<SanityPower>(
                    choiceContext,
                    enemy,
                    consumedCount,
                    base.Owner.Creature,
                    this,
                    false
                );
            }
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：从 1 变成 3
        base.DynamicVars["DiscardPicks"].UpgradeValueBy(2m);
    }
}