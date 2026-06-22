using Arknights_Mizuki.Scripts.Enchantments;
using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Relics;

[Pool(typeof(MzkRelicPool))]
public sealed class SwanSong : CustomRelicModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => HoverTipFactory.FromEnchantment<RevelationEnchantment>();

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override string PackedIconPath => "res://Arknights_Mizuki/images/relics/juechang.png";

    protected override string PackedIconOutlinePath => "res://Arknights_Mizuki/images/relics/juechang.png";

    protected override string BigIconPath => "res://Arknights_Mizuki/images/relics/juechang.png";

    public override async Task AfterObtained()
    {
        RevelationEnchantment enchantment = ModelDb.Enchantment<RevelationEnchantment>();
        List<CardModel> enchantableCards = PileType.Deck.GetPile(Owner).Cards
            .Where(card => enchantment.CanEnchant(card))
            .ToList();
        int selectCount = Math.Min(DynamicVars.Cards.IntValue, enchantableCards.Count);
        if (selectCount <= 0)
        {
            return;
        }

        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, selectCount);
        foreach (CardModel card in await CardSelectCmd.FromDeckForEnchantment(Owner, enchantment, 1, prefs))
        {
            CardCmd.Enchant(enchantment.ToMutable(), card, 1m);
            CardCmd.Preview(card);
        }
    }
}
