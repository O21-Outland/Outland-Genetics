using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace OutlandGenes
{
    public class Hediff_Implanted : HediffWithParents
	{
		public Pawn implanter;

		public GeneSet implantedGenes;

		public float progressSpeed = 1f;

        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref implanter, "implanter");
			Scribe_Values.Look(ref implantedGenes, "implantedGenes");
			Scribe_Values.Look(ref progressSpeed, "progressSpeed");
		}

        public float GestationProgress
		{
			get
			{
				return Severity;
			}
			private set
			{
				Severity = value;
			}
		}

		public override void Tick()
		{
			ageTicks++;
			float num = (PawnUtility.BodyResourceGrowthSpeed(pawn) / (10 * 60000f)) * progressSpeed;
			GestationProgress += num;
			if (!(GestationProgress >= 1f))
			{
				return;
			}
			else
			{
				StartLabor();
			}
			pawn.health.RemoveHediff(this);
		}

		public int TicksRemaining => Mathf.RoundToInt(((10 / progressSpeed) * 60000f) * (1f - GestationProgress));


		public override string LabelInBrackets => GestationProgress.ToStringPercent() + ", " + TicksRemaining.ToStringTicksToPeriod(false, true, true, false, false);

        public void StartLabor()
		{
			pawn.health.RemoveHediff(this);
		}

		public override void PreRemoved()
		{
			base.PreRemoved();


			Thing thing = DropSprog();
			SendLetter(thing);
		}

		public virtual void SendLetter(Thing thing)
        {
			ChoiceLetter_BabyBirth choiceLetter_BabyBirth = (ChoiceLetter_BabyBirth)LetterMaker.MakeLetter("OutlandGenes.SymbioteBirthLabel".Translate(), "OutlandGenes.SymbioteBirthDesc".Translate(pawn), LetterDefOf.BabyBirth, thing);
			choiceLetter_BabyBirth.Start();
			Find.LetterStack.ReceiveLetter(choiceLetter_BabyBirth);
		}

		public Pawn DropSprog()
		{
			Pawn host = pawn as Pawn;
			if (host.Spawned)
			{
				EffecterDefOf.Birth.Spawn(host, host.Map);
			}
			Pawn offspring = PawnGenerator.GeneratePawn(new PawnGenerationRequest(mother?.kindDef ?? PawnKindDefOf.Colonist, mother.Faction, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, PregnancyUtility.RandomLastName(mother, host as Pawn, father), null, null, null, forceNoIdeo: true, forceNoBackstory: false, forbidAnyTitle: false, false, null, forcedXenotype: XenotypeDefOf.Baseliner, forcedEndogenes: (geneSet.genes != null) ? geneSet.genes : PregnancyUtility.GetInheritedGenes(father, mother), forcedCustomXenotype: null, allowedXenotypes: null, forceBaselinerChance: 0f, developmentalStages: DevelopmentalStage.Newborn));
			if (mother.genes.UniqueXenotype)
			{
				offspring.genes.xenotypeName = mother.genes.xenotypeName;
				offspring.genes.iconDef = mother.genes.iconDef;
			}
			if (PregnancyUtility.TryGetInheritedXenotype(mother, father, out var xenotype))
			{
				offspring.genes?.SetXenotypeDirect(xenotype);
			}
			else if (PregnancyUtility.ShouldByHybrid(mother, father))
			{
				offspring.genes.hybrid = true;
				offspring.genes.xenotypeName = "Hybrid".Translate();
			}
			IntVec3? intVec = null;
			if (host != null && host.Spawned)
			{
				int? sleepingSlot;
				IntVec3 intVec2 = host.CurrentBed(out sleepingSlot)?.GetFootSlotPos(sleepingSlot.Value) ?? host.PositionHeld;
				intVec = CellFinder.RandomClosewalkCellNear(intVec2, host.Map, 1, delegate (IntVec3 cell)
				{
					if (cell != host.PositionHeld)
					{
						Building building = host.Map.edificeGrid[cell];
						if (building == null)
						{
							return true;
						}
						return building.def?.IsBed != true;
					}
					return false;
				});
				PregnancyUtility.SpawnBirthFilth(host, intVec2, ThingDefOf.Filth_AmnioticFluid, 1);
			}
			if (mother != null)
			{
				mother.health.AddHediff(HediffDefOf.Lactating);
			}
			if (offspring.RaceProps.IsFlesh)
			{
				if (mother != null)
				{
					offspring.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
				}
				if (father != null)
				{
					offspring.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
				}
				if (host != null && host != mother)
				{
					offspring.relations.AddDirectRelation(PawnRelationDefOf.ParentBirth, host);
				}
			}

			if (offspring.playerSettings != null && mother?.playerSettings != null)
			{
				offspring.playerSettings.allowedAreas = mother.playerSettings.allowedAreas;
			}
			if (offspring != null)
			{
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, host, offspring);
				offspring.mindState.SetAutofeeder(mother, AutofeedMode.Urgent);
			}
			if (!PawnUtility.TrySpawnHatchedOrBornPawn(offspring, host, intVec))
			{
				Find.WorldPawns.PassToWorld(offspring, PawnDiscardDecideMode.Discard);
			}
			mother?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.BabyBorn, offspring);
			host?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.BabyBorn, offspring);

			return offspring;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			foreach (Gizmo gizmo in base.GetGizmos())
			{
				yield return gizmo;
			}
			if (!DebugSettings.ShowDevGizmos)
			{
				yield break;
			}
			if (ModsConfig.BiotechActive && pawn.RaceProps.Humanlike && pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.PregnancyLabor) == null)
			{
				Command_Action command_Action2 = new Command_Action();
				command_Action2.defaultLabel = "DEV: Unleash Parasite";
				command_Action2.action = delegate
				{
					StartLabor();
				};
				yield return command_Action2;
			}
		}
	}
}
