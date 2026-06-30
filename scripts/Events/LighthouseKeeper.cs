using Arknights_Mizuki.Scripts.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace Arknights_Mizuki.Scripts.Events;

public sealed class LighthouseKeeper : CustomEventModel
{
    public override string? CustomInitialPortraitPath => "res://Arknights_Mizuki/images/events/Watcher.png";

    public override ActModel[] Acts => new[]
    {
        ModelDb.Act<Underdocks>()
    };

    public override bool IsAllowed(IRunState runState)
    {
        return runState is RunState state && state.CurrentActIndex == 0 && !state.VisitedEventIds.Contains(Id);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, Chat, OptionKey("CHAT")),
            new EventOption(this, WalkTogether, OptionKey("WALK_TOGETHER"))
        };
    }

    private string OptionKey(string option)
    {
        return $"{Id.Entry}.pages.INITIAL.options.{option}";
    }

    private async Task Chat()
    {
        CardCreationOptions options = CardCreationOptions
            .ForNonCombatWithUniformOdds(new CardPoolModel[] { Owner!.Character.CardPool }, card => card.Rarity == CardRarity.Rare)
            .WithFlags(CardCreationFlags.NoRarityModification);
        CardModel? card = CardFactory.CreateForReward(Owner, 1, options).FirstOrDefault()?.Card;
        if (card != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck));
        }

        SetEventFinished(PageDescription("CHAT"));
    }

    private async Task WalkTogether()
    {
        await GainKeyCharges(1);
        SetEventFinished(PageDescription("WALK_TOGETHER"));
    }

    private async Task GainKeyCharges(int amount)
    {
        Key? key = Owner!.GetRelic<Key>();
        if (key == null)
        {
            key = await RelicCmd.Obtain<Key>(Owner);
            amount--;
        }

        key.AddCharges(amount);
    }
}
