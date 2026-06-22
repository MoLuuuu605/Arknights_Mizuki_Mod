using Arknights_Mizuki.Scripts.Cards;
using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Arknights_Mizuki.Scripts.Potions;

[Pool(typeof(MzkPotionPool))]
public sealed class BottledSeaborn : CustomPotionModel
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.Self;

    public override string? CustomPackedImagePath => "res://Arknights_Mizuki/images/potions/BottledSeaborn.png";

    public override string? CustomPackedOutlinePath => CustomPackedImagePath;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new CardsVar(3)
    };

    public override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<BabyHs>()
    };

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        NCombatRoom.Instance?.PlaySplashVfx(Owner.Creature, new Color("29b8ff"));

        List<CardModel> cards = new();
        for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
        {
            cards.Add(Owner.Creature.CombatState.CreateCard<BabyHs>(Owner));
        }

        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(
            cards,
            PileType.Draw,
            Owner,
            CardPilePosition.Bottom));
    }
}
