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
public class EchoGenerateAuto : CustomCardModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型（防御牌是技能类型）
    private const CardType type = CardType.Skill;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Rare;
    // 目标类型（Self表示自己）
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    // 卡牌的基础属性（格挡值）
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
	{
		(DynamicVar)new CardsVar(2)
	};

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
	{
		HoverTipFactory.FromKeyword(Echo3.Echo)
	};
    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/EchoGenerateAuto.png";
    public EchoGenerateAuto() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    private int echo=0;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (this.Pile.Type == PileType.Hand){
            echo +=1 ;
            if(echo ==3)
            {
                DynamicVars.Cards.BaseValue += 1;
                echo=0;
            }
        }
    }

    // 打出时的效果逻辑
    Random rng = new Random();
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num=(int)this.DynamicVars.Cards.BaseValue;
        for(int i=0;i<num;i++)
        {
            int p =rng.Next(5);
            switch(p)
            {
                case 1:await CardPileCmd.Add(CombatState.CreateCard<Learn>(Owner), PileType.Discard,CardPilePosition.Random,null,true);break;
                case 2:await CardPileCmd.Add(CombatState.CreateCard<Share>(Owner), PileType.Discard,CardPilePosition.Random,null,true);break;
                case 3:await CardPileCmd.Add(CombatState.CreateCard<Shock>(Owner), PileType.Discard,CardPilePosition.Random,null,true);break;
                case 4:await CardPileCmd.Add(CombatState.CreateCard<Spray>(Owner), PileType.Discard,CardPilePosition.Random,null,true);break;
                default:await CardPileCmd.Add(CombatState.CreateCard<Explain>(Owner), PileType.Discard,CardPilePosition.Random,null,true);break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}