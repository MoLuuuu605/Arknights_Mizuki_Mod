using Arknights_Mizuki.Scripts.Cards;
using Arknights_Mizuki.Scripts.Powers;
using Arknights_Mizuki.Scripts.Utils;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace Arknights_Mizuki.Scripts.Enemies;

public sealed class Izumik : CustomMonsterModel
{
    private const string SpawnMoveId = "IZUMIK_SPAWN";
    private const string PressureMoveId = "IZUMIK_PRESSURE";
    private const string CorruptMoveId = "IZUMIK_CORRUPT";
    private const string CrushMoveId = "IZUMIK_CRUSH";
    private const string PhaseTwoStartMoveId = "IZUMIK_INTERPRET";
    private const string PhaseTwoMultiMoveId = "IZUMIK_PHASE_2_MULTI";
    private const string PhaseTwoCorruptMoveId = "IZUMIK_PHASE_2_CORRUPT";
    private const string PhaseTwoCrushMoveId = "IZUMIK_PHASE_2_CRUSH";
    private const string PhaseTwoSpawnMoveId = "IZUMIK_PHASE_2_SPAWN";

    private int absorbedOffspring;
    private bool phaseTwoStarted;

    public override int MinInitialHp => Asc(700, 660);
    public override int MaxInitialHp => Asc(780, 720);
    protected override string VisualsPath => CustomVisualPath;
    public override string CustomVisualPath => "res://Arknights_Mizuki/enemies/izumik/izumik.tscn";
    public override bool ShouldReceiveCombatHooks => true;
    public override NCreatureVisuals? CreateCustomVisuals() => NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    public override CreatureAnimator GenerateAnimator(MegaSprite controller) => SpineAnimatorFactory.Create(controller, cast: "Skill_1_Begin", buff: "Skill_2", summon: "Skill_1_Begin");

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<IzumikPhaseShiftPower>(
            new ThrowingPlayerChoiceContext(),
            Creature,
            Math.Ceiling(Creature.MaxHp / 2m),
            Creature,
            null,
            false);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        absorbedOffspring = 0;
        phaseTwoStarted = false;

        MoveState spawn = new(SpawnMoveId, _ => SummonOffspring(2, 14), new SummonIntent(), new BuffIntent())
        {
            FollowUpStateId = PressureMoveId
        };
        MoveState pressure = new(PressureMoveId, targets => AttackAndSummon(targets, Asc(18, 14), 2, 1), new MultiAttackIntent(Asc(18, 14), 2), new SummonIntent())
        {
            FollowUpStateId = CorruptMoveId
        };
        MoveState corrupt = new(CorruptMoveId, targets => CorruptAndSummon(targets, weak: 2, vulnerable: 2, sanity: Asc(3, 2), summonCount: 1), new StatusIntent(3), new SummonIntent())
        {
            FollowUpStateId = CrushMoveId
        };
        MoveState crush = new(CrushMoveId, targets => AttackAndSummon(targets, Damage(22, 30, 26, 34), 1, 1), new SingleAttackIntent(() => DamageIntent(22, 30, 26, 34)), new SummonIntent())
        {
            FollowUpStateId = PhaseTwoStartMoveId
        };
        MoveState phaseTwoStart = new(PhaseTwoStartMoveId, _ => EnterPhaseTwo(), new BuffIntent())
        {
            FollowUpStateId = PhaseTwoMultiMoveId
        };
        MoveState phaseTwoMulti = new(PhaseTwoMultiMoveId, targets => AttackAndSanity(targets, Asc(18, 15), 2, Asc(3, 2)), new MultiAttackIntent(Asc(18, 15), 2), new DebuffIntent(false))
        {
            FollowUpStateId = PhaseTwoCorruptMoveId
        };
        MoveState phaseTwoCorrupt = new(PhaseTwoCorruptMoveId, targets => PhaseTwoCorruption(targets), new StatusIntent(5),new DebuffIntent(false))
        {
            FollowUpStateId = PhaseTwoCrushMoveId
        };
        MoveState phaseTwoCrush = new(PhaseTwoCrushMoveId, targets => Attack(targets, Damage(34, 42, 38, 48)), new SingleAttackIntent(() => DamageIntent(34, 42, 38, 48)),new StatusIntent(1))
        {
            FollowUpStateId = PhaseTwoSpawnMoveId
        };
        MoveState phaseTwoSpawn = new(PhaseTwoSpawnMoveId, _ => SummonAndStrengthen(), new SummonIntent(), new BuffIntent())
        {
            FollowUpStateId = PhaseTwoMultiMoveId
        };

        return new MonsterMoveStateMachine(
            new MonsterState[]
            {
                spawn,
                pressure,
                corrupt,
                crush,
                phaseTwoStart,
                phaseTwoMulti,
                phaseTwoCorrupt,
                phaseTwoCrush,
                phaseTwoSpawn
            },
            spawn);
    }

    public void QueuePhaseTwoIntent()
    {
        if (phaseTwoStarted || !Creature.IsAlive)
            return;

        SetMoveImmediate(MoveStateMachine.States[PhaseTwoStartMoveId] as MoveState, true);
    }

    public async Task AbsorbOffspring(PlayerChoiceContext choiceContext, Creature offspring)
    {
        if (!Creature.IsAlive)
            return;

        await CreatureCmd.TriggerAnim(Creature, "Buff", 0.2f);
        absorbedOffspring++;
        await CreatureCmd.Heal(Creature, Asc(14, 10), true);
        await PowerCmd.Apply<IzumikEvolutionPower>(choiceContext, Creature, 1, Creature, null, false);

        if (absorbedOffspring % 3 == 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Creature, absorbedOffspring / 3, Creature, null, false);
        }
    }

    private async Task SummonOffspring(int count, decimal block)
    {
        if (count > 0)
            await CreatureCmd.TriggerAnim(Creature, "Summon", 0.25f);

        for (int i = 0; i < count; i++)
        {
            if (Creature.CombatState.Enemies.Count(enemy => enemy.IsAlive && enemy.Monster is IzumikOffspring) >= 3)
                break;

            await CreatureCmd.Add<IzumikOffspring>(Creature.CombatState);
            SeparateOffspringVisuals();
        }

        if (block > 0)
            await CreatureCmd.GainBlock(Creature, new BlockVar(block, ValueProp.Move), null, false);
    }

    private void SeparateOffspringVisuals()
    {
        NCombatRoom? room = NCombatRoom.Instance;
        if (room == null)
            return;

        var bossNode = room.GetCreatureNode(Creature);
        if (bossNode == null)
            return;

        var offspringNodes = Creature.CombatState.Enemies
            .Where(enemy => enemy.IsAlive && enemy.Monster is IzumikOffspring)
            .Select(enemy => room.GetCreatureNode(enemy))
            .Where(node => node != null)
            .ToList();

        if (offspringNodes.Count == 0)
            return;

        float widestOffspring = offspringNodes.Max(node => node!.Visuals.Bounds.Size.X);
        float spacing = Math.Max(120f, widestOffspring + 35f);
        float totalWidth = spacing * (offspringNodes.Count - 1);
        float bossLeft = bossNode.Position.X - bossNode.Visuals.Bounds.Size.X * 0.5f;
        float startX = Math.Max(80f, bossLeft - 90f - totalWidth);

        for (int i = 0; i < offspringNodes.Count; i++)
        {
            var node = offspringNodes[i]!;
            float yOffset = i % 2 == 0 ? 65f : 10f;
            node.Position = new Vector2(startX + spacing * i, bossNode.Position.Y + yOffset);
        }
    }

    private async Task AttackAndSummon(IReadOnlyList<Creature> targets, int damage, int hits, int summonCount)
    {
        Creature? target = targets.FirstOrDefault();
        if (target != null)
        {
            await CreatureCmd.TriggerAnim(Creature, "Attack", 0.25f);
            for (int i = 0; i < hits; i++)
            {
                await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, damage, ValueProp.Move, Creature);
            }
        }

        await SummonOffspring(summonCount, 0);
    }

    private async Task Attack(IReadOnlyList<Creature> targets, int damage)
    {
        Creature? target = targets.FirstOrDefault();
        if (target == null)
            return;

        await CreatureCmd.TriggerAnim(Creature, "Attack", 0.25f);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, damage, ValueProp.Move, Creature);
        await AddFrozen(target);
    }

    private async Task AttackAndSanity(IReadOnlyList<Creature> targets, int damage, int hits, decimal sanity)
    {
        await AttackAndSummon(targets, damage, hits, 0);
        await CorruptPlayer(targets, weak: 0, vulnerable: 0, sanity);
    }

    private async Task CorruptPlayer(IReadOnlyList<Creature> targets, decimal weak, decimal vulnerable, decimal sanity)
    {
        Creature? target = targets.FirstOrDefault();
        if (target == null)
            return;

        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.25f);
        ThrowingPlayerChoiceContext choiceContext = new();
        if (weak > 0)
            await PowerCmd.Apply<WeakPower>(choiceContext, target, weak, Creature, null, false);
        if (vulnerable > 0)
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, vulnerable, Creature, null, false);
        if (sanity > 0)
            await PowerCmd.Apply<SanityPower>(choiceContext, target, sanity, Creature, null, false);
    }

    private async Task CorruptAndSummon(IReadOnlyList<Creature> targets, decimal weak, decimal vulnerable, decimal sanity, int summonCount)
    {
        await CorruptPlayer(targets, weak, vulnerable, sanity);
        await SummonOffspring(summonCount, 0);
        for (int i = 0; i < targets.Count; i++)
        {
            CardModel frozen = Creature.CombatState.CreateCard<Frozen>(targets[i].Player);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                frozen,
                PileType.Discard,
                targets[i].Player,
                CardPilePosition.Random));
        }
    }

    private async Task PhaseTwoCorruption(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "Cast", 0.25f);
        Creature? target = targets.FirstOrDefault();
        if (target?.Player != null)
        {
            for (int i = 0; i < 2; i++)
            {
                CardModel slimed = Creature.CombatState.CreateCard<Slimed>(target.Player);
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                    slimed,
                    PileType.Discard,
                    target.Player,
                    CardPilePosition.Random));
            }
            CardModel frozen = Creature.CombatState.CreateCard<Frozen>(target.Player);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                frozen,
                PileType.Discard,
                target.Player,
                CardPilePosition.Random));
            CardModel hazed = Creature.CombatState.CreateCard<Haze>(target.Player);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                hazed,
                PileType.Discard,
                target.Player,
                CardPilePosition.Random));
        }

        await CorruptPlayer(targets, weak: 0, vulnerable: 2, sanity: Asc(4, 3));
    }

    private async Task SummonAndStrengthen()
    {
        await SummonOffspring(2, Asc(18, 14));
        await CreatureCmd.TriggerAnim(Creature, "Buff", 0.2f);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, Asc(3, 2), Creature, null, false);
    }

    private async Task EnterPhaseTwo()
    {
        if (phaseTwoStarted)
            return;

        phaseTwoStarted = true;
        await CreatureCmd.TriggerAnim(Creature, "Buff", 0.35f);

        foreach (Creature offspring in Creature.CombatState.Enemies.Where(enemy => enemy.IsAlive && enemy.Monster is IzumikOffspring).ToList())
        {
            await AbsorbOffspring(new ThrowingPlayerChoiceContext(), offspring);
            if (offspring.IsAlive)
            {
                await CreatureCmd.Damage(
                    new ThrowingPlayerChoiceContext(),
                    offspring,
                    offspring.CurrentHp,
                    ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
                    Creature);
            }
        }

        foreach (PowerModel power in Creature.Powers.ToList())
        {
            if (power.Type == PowerType.Debuff)
                await PowerCmd.Remove(power);
        }

        decimal targetHp = Creature.MaxHp * 0.6m;
        if (Creature.CurrentHp < targetHp)
            await CreatureCmd.Heal(Creature, targetHp - Creature.CurrentHp, true);

        if (absorbedOffspring > 0)
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, Math.Max(1, absorbedOffspring / 2), Creature, null, false);
    }

    private int Damage(int normalMin, int normalMax, int ascMin, int ascMax)
    {
        return RunRng.MonsterAi.NextInt(Asc(ascMin, normalMin), Asc(ascMax, normalMax) + 1);
    }

    private int DamageIntent(int normalMin, int normalMax, int ascMin, int ascMax)
    {
        return (Asc(ascMin, normalMin) + Asc(ascMax, normalMax)) / 2;
    }

    private static int Asc(int highAscensionValue, int normalValue)
    {
        return AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, highAscensionValue, normalValue);
    }
    private async Task AddFrozen(Creature? target)
    {
        if (target?.Player != null)
        {
            CardModel slimed = Creature.CombatState.CreateCard<Frozen>(target.Player);
            CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(
                slimed,
                PileType.Discard,
                target.Player,
                CardPilePosition.Bottom));
        }
    }

}
