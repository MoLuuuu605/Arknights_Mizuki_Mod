using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.HoverTips;

using Arknights_Mizuki.Scripts.Pools;
using Arknights_Mizuki.Scripts.Powers;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public class MzkSanityBurst : CustomCardModel
{
    private const int energyCost = 3;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[1]
    {
        (DynamicVar)new PowerVar<SanityBurstPower>(1m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[2]
    {
        HoverTipFactory.FromPower<SanityBurstPower>(),
        HoverTipFactory.FromPower<SanityUnlimitPower>()
    };

    public override string PortraitPath => $"res://Arknights_Mizuki/images/cards/burst.png";

    public MzkSanityBurst() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SanityBurstPower>(
            choiceContext,
            ((CardModel)this).Owner.Creature,
            ((DynamicVar)((CardModel)this).DynamicVars["SanityBurstPower"]).BaseValue,
            ((CardModel)this).Owner.Creature,
            (CardModel)(object)this,
            false
        );
    }

    protected override void OnUpgrade()
    {
        // 升级后降为1费
        ((CardModel)this).EnergyCost.UpgradeBy(-1);
    }
}