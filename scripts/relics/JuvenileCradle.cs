using Arknights_Mizuki.Scripts.Cards;
using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Relics;

[Pool(typeof(MzkRelicPool))]
public sealed class JuvenileCradle : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override string PackedIconPath => "res://Arknights_Mizuki/images/relics/juvenle_cardle.png";

    protected override string PackedIconOutlinePath => "res://Arknights_Mizuki/images/relics/juvenle_cardle.png";

    protected override string BigIconPath => "res://Arknights_Mizuki/images/relics/juvenle_cardle.png";

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != Owner || card is not BabyHs || card.IsUpgraded)
        {
            return Task.CompletedTask;
        }

        Flash();
        CardCmd.Upgrade(card);
        return Task.CompletedTask;
    }
}
