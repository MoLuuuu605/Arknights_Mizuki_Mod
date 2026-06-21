using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

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
        if (cardPlay.Card.Owner != Owner)
            return;

        if (this.Pile.Type == PileType.Hand){
            echo +=1 ;
            if(echo ==3)
            {
                DynamicVars.Cards.BaseValue += 1;
                echo=0;
            }
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int num=(int)this.DynamicVars.Cards.BaseValue;
        for(int i=0;i<num;i++)
        {
            CardModel generatedCard = Owner.RunState.Rng.CombatCardSelection.NextItem(new CardModel[]
            {
                CombatState.CreateCard<Learn>(Owner),
                CombatState.CreateCard<Share>(Owner),
                CombatState.CreateCard<Shock>(Owner),
                CombatState.CreateCard<Spray>(Owner),
                CombatState.CreateCard<WaterSheild>(Owner),
                CombatState.CreateCard<Explain>(Owner),
            });
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(generatedCard, PileType.Discard, Owner, CardPilePosition.Random));
        }
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}


