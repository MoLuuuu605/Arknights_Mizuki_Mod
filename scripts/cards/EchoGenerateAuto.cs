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

// 娉ㄥ唽鍗＄墝鍒?MzkCardPool
[Pool(typeof(MzkCardPool))]
public class EchoGenerateAuto : CustomCardModel
{
    // 鍩虹鑰楄兘
    private const int energyCost = 1;
    // 鍗＄墝绫诲瀷锛堥槻寰＄墝鏄妧鑳界被鍨嬶級
    private const CardType type = CardType.Skill;
    // 鍗＄墝绋€鏈夊害
    private const CardRarity rarity = CardRarity.Rare;

    private const TargetType targetType = TargetType.Self;

    // 鏄惁鍦ㄥ崱鐗屽浘閴翠腑鏄剧ず
    private const bool shouldShowInCardLibrary = true;

    // 鍗＄墝鐨勫熀纭€灞炴€э紙鏍兼尅鍊硷級
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

    Random rng = new Random();
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num=(int)this.DynamicVars.Cards.BaseValue;
        for(int i=0;i<num;i++)
        {
            int p =rng.Next(5);
            switch(p)
            {
                case 1:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Learn>(Owner), PileType.Discard,Owner,CardPilePosition.Random));break;
                case 2:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Share>(Owner), PileType.Discard,Owner,CardPilePosition.Random));break;
                case 3:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Shock>(Owner), PileType.Discard,Owner,CardPilePosition.Random));break;
                case 4:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Spray>(Owner), PileType.Discard,Owner,CardPilePosition.Random));break;
                case 5:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<WaterSheild>(Owner),PileType.Discard,Owner,CardPilePosition.Random));break;
                default:CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Explain>(Owner), PileType.Discard,Owner,CardPilePosition.Random));break;
            }
        }
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}


