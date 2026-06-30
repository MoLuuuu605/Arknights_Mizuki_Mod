using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Arknights_Mizuki.Scripts.Relics;

[Pool(typeof(MzkRelicPool))]
public sealed class DriftingCoffer : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(2),
        new GoldVar(30)
    };

    public override string PackedIconPath => "res://Arknights_Mizuki/images/relics/drifting_coffer.png";

    protected override string PackedIconOutlinePath => "res://Arknights_Mizuki/images/relics/drifting_coffer_outline.png";

    protected override string BigIconPath => "res://Arknights_Mizuki/images/relics/drifting_coffer.png";

    public override async Task AfterObtained()
    {
        Key? key = Owner.GetRelic<Key>();
        if (key == null || key.ChargesRemaining <= 0)
        {
            await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
            return;
        }

        key.ChargesRemaining--;
        Flash();

        List<Reward> rewards = new();
        CardCreationOptions options = new(
            new CardPoolModel[] { Owner.Character.CardPool },
            CardCreationSource.Other,
            CardRarityOddsType.RegularEncounter);

        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            rewards.Add(new CardReward(options, 3, Owner));
        }

        await RewardsCmd.OfferCustom(Owner, rewards);
    }
}
