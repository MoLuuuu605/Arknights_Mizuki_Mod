using Arknights_Mizuki.Scripts.Relics;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Arknights_Mizuki.Scripts.Events;

public sealed class Sublimation : CustomEventModel
{
    private const decimal DeterminationHpThreshold = 0.8m;
    private const decimal ObservationHpThreshold = 0.5m;

    public override string? CustomInitialPortraitPath => "res://Arknights_Mizuki/images/events/shenghua.png";

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DynamicVar("MaxHpGain", 20m)
    };

    public override ActModel[] Acts => new[]
    {
        ModelDb.Act<Glory>()
    };

    public override bool IsAllowed(IRunState runState)
    {
        return runState is RunState state && state.CurrentActIndex == 2 && !state.VisitedEventIds.Contains(Id);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, CanChooseDetermination() ? ChooseDetermination : null, OptionKey("CHOOSE_DETERMINATION")).WithRelic<Determination>(Owner),
            new EventOption(this, CanChooseObservation() ? ChooseObservation : null, OptionKey("CHOOSE_OBSERVATION")).WithRelic<Observation>(Owner),
            new EventOption(this, ChooseHerd, OptionKey("CHOOSE_HERD"))
        };
    }

    private string OptionKey(string option)
    {
        return $"{Id.Entry}.pages.INITIAL.options.{option}";
    }

    private bool CanChooseDetermination()
    {
        return Owner != null && Owner.Creature.CurrentHp > Owner.Creature.MaxHp * DeterminationHpThreshold;
    }

    private bool CanChooseObservation()
    {
        return Owner != null && Owner.Creature.CurrentHp > Owner.Creature.MaxHp * ObservationHpThreshold;
    }

    private async Task ChooseDetermination()
    {
        await RelicCmd.Obtain<Determination>(Owner!);
        SetEventFinished(PageDescription("DETERMINATION"));
    }

    private async Task ChooseObservation()
    {
        await RelicCmd.Obtain<Observation>(Owner!);
        SetEventFinished(PageDescription("OBSERVATION"));
    }

    private async Task ChooseHerd()
    {
        await CardPileCmd.AddCurseToDeck<Doubt>(Owner!);
        await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars["MaxHpGain"].BaseValue);
        SetEventFinished(PageDescription("HERD"));
    }
}
