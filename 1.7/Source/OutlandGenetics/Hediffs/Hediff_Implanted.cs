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
			bool hostWasLactating = false;
			if(host.health.hediffSet.HasHediff(HediffDefOf.Lactating))
			{
				hostWasLactating = true;
			}
            Precept_Ritual ritual = Faction.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.ChildBirth) as Precept_Ritual;
            HumanOvum ovum = ThingMaker.MakeThing(ThingDefOf.HumanOvum) as HumanOvum;
            ovum.TryGetComp<CompHasPawnSources>().AddSource(Mother);
            HumanEmbryo embryo = ovum.ProduceEmbryo(Father) as HumanEmbryo;
            Thing thing = PregnancyUtility.ApplyBirthOutcome(RitualOutcomeEffectDefOf.ChildBirth.BestOutcome, 1f, ritual, embryo?.GeneSet?.GenesListForReading, Mother, host, Father, preventLetter: true);
			embryo.Destroy();
			Pawn offspring = thing as Pawn;
			//clean up after stuff rimworld does here that we don't want
			if (!hostWasLactating)
			{
				Hediff lactationHediff;
				if (host.health.hediffSet.TryGetHediff(HediffDefOf.Lactating, out lactationHediff))
				{
					host.health.RemoveHediff(lactationHediff);
				}
			}
			offspring.mindState.SetAutofeeder(host, AutofeedMode.Childcare);
			thing.SetFaction(Mother.Faction);

			if (Mother != null)
			{
				Mother.health.AddHediff(HediffDefOf.Lactating);
			}
			if (offspring != null)
			{
				TaleRecorder.RecordTale(TaleDefOf.GaveBirth, host, offspring);
				offspring.mindState.SetAutofeeder(Mother, AutofeedMode.Urgent);
			}
			Mother?.needs?.mood?.thoughts?.memories?.TryGainMemory(ThoughtDefOf.BabyBorn, offspring);
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
