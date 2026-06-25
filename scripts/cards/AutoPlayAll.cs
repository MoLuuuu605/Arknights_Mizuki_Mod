using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

using Arknights_Mizuki.Scripts.Pools;
using MegaCrit.Sts2.Core.HoverTips;
using System.Runtime.InteropServices.Marshalling;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Cards;

// 娉ㄥ唽鍗＄墝鍒?MzkCardPool
[Pool(typeof(MzkCardPool))]
public class AutoPlayAll : CustomCardModel
{
    // 鍩虹鑰楄兘
    private const int energyCost = 1;
    // 鍗＄墝绫诲瀷锛堥槻寰＄墝鏄妧鑳界被鍨嬶級
    private const CardType type = CardType.Skill;
    // 鍗＄墝绋€鏈夊害
    private const CardRarity rarity = CardRarity.Uncommon;

    private const TargetType targetType = TargetType.Self;
    // 鐩爣绫诲瀷锛圫elf琛ㄧず鑷繁锛?    private const TargetType targetType = TargetType.Self;
    // 鏄惁鍦ㄥ崱鐗屽浘閴翠腑鏄剧ず
    private const bool shouldShowInCardLibrary = true;

    // 鍗＄墝鐨勫熀纭€灞炴€э紙鏍兼尅鍊硷級
    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[0];
    public override IEnumerable<CardKeyword> CanonicalKeywords => (IEnumerable<CardKeyword>)(object)new CardKeyword[1] {CardKeyword.Exhaust};
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

    // 鎵撳嚭鏃剁殑鏁堟灉閫昏緫
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Learn>(Owner), PileType.Discard,Owner,CardPilePosition.Random));
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Share>(Owner), PileType.Discard,Owner,CardPilePosition.Random));
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<Shock>(Owner), PileType.Discard,Owner,CardPilePosition.Random));
        await CardPileCmd.AddGeneratedCardToCombat(CombatState.CreateCard<AutoPlayAll>(Owner),PileType.Hand,Owner);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
}


