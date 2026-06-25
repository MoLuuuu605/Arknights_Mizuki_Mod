using Arknights_Mizuki.Scripts.Pools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Cards;

[Pool(typeof(MzkCardPool))]
public sealed class SeabornFinale : CustomCardModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars => (IEnumerable<DynamicVar>)(object)new DynamicVar[2]
    {
        (DynamicVar)new DamageVar(0m, ValueProp.Move),
        (DynamicVar)new IntVar("Hits", 3m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => (IEnumerable<IHoverTip>)(object)new IHoverTip[1]
    {
        HoverTipFactory.FromCard<BabyHs>()
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[1] { CardKeyword.Exhaust };

    public override string PortraitPath => "res://Arknights_Mizuki/images/cards/SeabornFinale.png";

    public SeabornFinale() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        List<CardModel> babies = PileType.Draw.GetPile(Owner).Cards
            .Concat(PileType.Discard.GetPile(Owner).Cards)
            .Where(card => card is BabyHs)
            .ToList();

        decimal damage = babies.Count;
        var enemies = CombatState.GetOpponentsOf(Owner.Creature)
            .Where(enemy => enemy.IsAlive)
            .ToList();

        for (int i = 0; i < DynamicVars["Hits"].IntValue && enemies.Count > 0; i++)
        {
            Creature target = Owner.RunState.Rng.CombatCardSelection.NextItem(enemies);
            await DamageCmd.Attack(damage)
                .FromCard(this)
                .Targeting(target)
                .Execute(choiceContext);
            enemies = enemies.Where(enemy => enemy.IsAlive).ToList();
        }

        foreach (CardModel baby in babies)
        {
            await CardCmd.Exhaust(choiceContext, baby);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Hits"].UpgradeValueBy(1m);
    }
}
