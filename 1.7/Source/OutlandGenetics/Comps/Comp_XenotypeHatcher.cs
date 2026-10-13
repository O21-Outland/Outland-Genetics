using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    public class Comp_XenotypeHatcher : ThingComp
    {
        public CompProperties_XenotypeHatcher Props => (CompProperties_XenotypeHatcher)props;

		private float gestateProgress;

		public Pawn hatcheeParent;

		public Pawn otherParent;

		public float progressSpeed = 1f;

		public Faction hatcheeFaction = Faction.OfPlayer;

		private CompTemperatureRuinable FreezerComp => parent.GetComp<CompTemperatureRuinable>();

		public bool TemperatureDamaged => FreezerComp != null && FreezerComp.Ruined;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref gestateProgress, "gestateProgress", 0f);
			Scribe_References.Look(ref hatcheeParent, "hatcheeParent");
			Scribe_References.Look(ref otherParent, "otherParent");
			Scribe_References.Look(ref hatcheeFaction, "hatcheeFaction");
			Scribe_Values.Look(ref progressSpeed, "progressSpeed");
		}

		public override void CompTick()
		{
			if (!TemperatureDamaged)
			{
				float num = (1f / (Props.hatcherDaystoHatch * 60000f)) * progressSpeed;
				gestateProgress += num;
				if (gestateProgress > 1f)
				{
					Hatch();
				}
			}
		}

		public void Hatch()
		{
			try
			{
				PawnGenerationRequest request = new PawnGenerationRequest(Props.hatcherPawn, hatcheeFaction, PawnGenerationContext.NonPlayer, -1, false, true, true, false, false, -1, false, true, false, false, false, false, false, false, false, 0, 0, null, -1, null, null, null, null, null, 0, 0, null, null, null, null, null, false, developmentalStages: DevelopmentalStage.Newborn, allowedXenotypes: new List<XenotypeDef>() { Props.hatcherXenotype });
				for (int i = 0; i < parent.stackCount; i++)
				{
					Pawn pawn = PawnGenerator.GeneratePawn(request);
					if (PawnUtility.TrySpawnHatchedOrBornPawn(pawn, (Thing)parent))
					{
						if (pawn != null)
						{
							if (hatcheeParent != null)
							{
								if (pawn.playerSettings != null && hatcheeParent.playerSettings != null && hatcheeParent.Faction == hatcheeFaction)
								{
									pawn.playerSettings.allowedAreas = hatcheeParent.playerSettings.allowedAreas;
								}
								if (pawn.RaceProps.IsFlesh)
								{
									pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, hatcheeParent);
								}
							}
							if (otherParent != null && (hatcheeParent == null || hatcheeParent.gender != otherParent.gender) && pawn.RaceProps.IsFlesh)
							{
								pawn.relations.AddDirectRelation(PawnRelationDefOf.Parent, otherParent);
							}
							SendLetter(pawn);
						}
						if (parent.Spawned)
						{
							FilthMaker.TryMakeFilth(parent.Position, parent.Map, ThingDefOf.Filth_AmnioticFluid, 1, FilthSourceFlags.None);
						}
					}
					else
					{
						Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
					}
				}
			}
			finally
			{
				parent.Destroy();
			}
		}

		public void SendLetter(Pawn pawn)
        {
			if(pawn == null)
            {
				return;
            }
			ChoiceLetter_BabyBirth choiceLetter = (ChoiceLetter_BabyBirth)LetterMaker.MakeLetter("Outland.XenotypeEggHatchLabel".Translate(Props.hatcherXenotype.LabelCap), "Outland.XenotypeEggHatchLetter".Translate(Props.hatcherXenotype.LabelCap), LetterDefOf.BabyBirth, (TargetInfo)pawn);
			choiceLetter.Start();
			Find.LetterStack.ReceiveLetter(choiceLetter);
		}

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!base.CompGetGizmosExtra().EnumerableNullOrEmpty())
            {
				foreach(Gizmo gizzy in base.CompGetGizmosExtra())
                {
					yield return gizzy;
                }
            }
            if (DebugSettings.ShowDevGizmos)
			{
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "DEV: Hatch";
				command_Action.action = delegate
				{
					Hatch();
				};
				yield return command_Action;
			}
        }

        public override bool AllowStackWith(Thing other)
        {
			return false;
        }

        public override void PreAbsorbStack(Thing otherStack, int count)
		{
			float t = (float)count / (float)(parent.stackCount + count);
			float b = ((ThingWithComps)otherStack).GetComp<Comp_XenotypeHatcher>().gestateProgress;
			gestateProgress = Mathf.Lerp(gestateProgress, b, t);
		}

		public override void PostSplitOff(Thing piece)
		{
			Comp_XenotypeHatcher comp = ((ThingWithComps)piece).GetComp<Comp_XenotypeHatcher>();
			comp.gestateProgress = gestateProgress;
			comp.hatcheeParent = hatcheeParent;
			comp.otherParent = otherParent;
			comp.hatcheeFaction = hatcheeFaction;
		}

		public override void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
		{
			base.PrePreTraded(action, playerNegotiator, trader);
			switch (action)
			{
				case TradeAction.PlayerBuys:
					hatcheeFaction = Faction.OfPlayer;
					break;
				case TradeAction.PlayerSells:
					hatcheeFaction = trader.Faction;
					break;
			}
		}

		public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
		{
			base.PostPostGeneratedForTrader(trader, forTile, forFaction);
			hatcheeFaction = forFaction;
		}

		public override string CompInspectStringExtra()
		{
			if (!TemperatureDamaged)
			{
				return "EggProgress".Translate() + ": " + gestateProgress.ToStringPercent() + "\n" + "HatchesIn".Translate() + ": " + "PeriodDays".Translate(((Props.hatcherDaystoHatch / progressSpeed) * (1f - gestateProgress)).ToString("F1"));
			}
			return null;
		}
	}
}
