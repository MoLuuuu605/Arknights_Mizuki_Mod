using Arknights_Mizuki.Scripts.Acts;
using Arknights_Mizuki.Scripts.Relics;
using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Arknights_Mizuki.Scripts.Ancients;

public sealed class LastTidewatcher : CustomAncientModel
{
    private const string Key = "ARKNIGHTS_MIZUKI-LAST_TIDEWATCHER";

    protected override OptionPools MakeOptionPools { get; } = new OptionPools(MakePool(Array.Empty<AncientOption>()));

    public override Color ButtonColor => new Color(0.05f, 0.09f, 0.13f, 0.8f);

    public override Color DialogueColor => new Color("8ec8de");

    public override string? CustomScenePath => "res://Arknights_Mizuki/scenes/events/last_tidewatcher.tscn";

    public override string? CustomMapIconPath => "res://Arknights_Mizuki/images/map/ancients/last_tidewatcher.png";

    public override string? CustomMapIconOutlinePath => "res://Arknights_Mizuki/images/map/ancients/last_tidewatcher_outline.png";

    public override string? CustomRunHistoryIconPath => "res://Arknights_Mizuki/images/ui/run_history/last_tidewatcher.png";

    public override string? CustomRunHistoryIconOutlinePath => "res://Arknights_Mizuki/images/ui/run_history/last_tidewatcher_outline.png";

    public override IEnumerable<EventOption> AllPossibleOptions => MakeOptions();

    public override bool IsValidForAct(ActModel act)
    {
        return act is EvolutionSingularityAct;
    }

    public override bool ShouldForceSpawn(ActModel act, AncientEventModel ancient)
    {
        return act is EvolutionSingularityAct;
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        if (Owner == null)
            return MakeOptions().Take(3).ToList();

        List<EventOption> options = MakeOptions().ToList();
        for (int i = options.Count - 1; i > 0; i--)
        {
            int swapIndex = Owner.RunState.Rng.Niche.NextInt(i + 1);
            (options[i], options[swapIndex]) = (options[swapIndex], options[i]);
        }

        return options.Take(3).ToList();
    }

    protected override AncientDialogueSet DefineDialogues()
    {
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(""),
            CharacterDialogues = new Dictionary<string, IReadOnlyList<AncientDialogue>>(),
            AgnosticDialogues = new AncientDialogue[]
            {
                new AncientDialogue("")
            }
        };
    }

    private IReadOnlyList<EventOption> MakeOptions()
    {
        return new EventOption[]
        {
            new EventOption(this, LoseMaxHpForFiveRelics, $"{Key}.pages.INITIAL.options.FIVE_RELICS")
                .ThatDecreasesMaxHp(GetMaxHpCost()),
            new EventOption(this, ObtainLantern, $"{Key}.pages.INITIAL.options.LANTERN")
                .WithRelic<TidewatcherLantern>(Owner),
            new EventOption(this, LoseGoldForHeart, $"{Key}.pages.INITIAL.options.HEART")
                .WithRelic<DeepBlueHeart>(Owner),
            new EventOption(this, ObtainThreeRelics, $"{Key}.pages.INITIAL.options.THREE_RELICS"),
            new EventOption(this, ObtainSwanSong, $"{Key}.pages.INITIAL.options.SWAN_SONG")
                .WithRelic<SwanSong>(Owner),
            new EventOption(this, ObtainOceanPulse, $"{Key}.pages.INITIAL.options.OCEAN_PULSE")
                .WithRelic<OceanPulse>(Owner)
        };
    }

    private decimal GetMaxHpCost()
    {
        if (Owner == null)
            return 1m;

        return Math.Max(1m, Math.Ceiling(Owner.Creature.MaxHp * 0.2m));
    }

    private decimal GetMaxHpCost30()
    {
        if (Owner == null)
            return 1m;

        return Math.Max(1m, Math.Ceiling(Owner.Creature.MaxHp * 0.3m));
    }
    private async Task LoseMaxHpForFiveRelics()
    {
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner!.Creature, GetMaxHpCost(), false);
        await ObtainRandomRelics(5);
        Done();
    }

    private async Task ObtainLantern()
    {
        await RelicCmd.Obtain(ModelDb.Relic<TidewatcherLantern>().ToMutable(), Owner!);
        Done();
    }

    private async Task LoseGoldForHeart()
    {
        await PlayerCmd.LoseGold(Owner!.Gold, Owner, GoldLossType.Spent);
        await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner!.Creature, GetMaxHpCost30(), false);
        await RelicCmd.Obtain(ModelDb.Relic<DeepBlueHeart>().ToMutable(), Owner);
        Done();
    }

    private async Task ObtainSwanSong()
    {
        await RelicCmd.Obtain(ModelDb.Relic<SwanSong>().ToMutable(), Owner!);
        Done();
    }

    private async Task ObtainOceanPulse()
    {
        await RelicCmd.Obtain(ModelDb.Relic<OceanPulse>().ToMutable(), Owner!);
        Done();
    }

    private async Task ObtainThreeRelics()
    {
        await ObtainRandomRelics(3);
        Done();
    }

    private async Task ObtainRandomRelics(int count)
    {
        for (int i = 0; i < count; i++)
        {
            RelicModel relic = RelicFactory.PullNextRelicFromFront(Owner!).ToMutable();
            await RelicCmd.Obtain(relic, Owner!);
        }
    }
}
