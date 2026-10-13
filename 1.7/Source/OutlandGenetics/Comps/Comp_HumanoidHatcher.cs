using RimWorld;
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
    public class Comp_HumanoidHatcher : ThingComp
    {
        public float gestateProgress;

        public HumanEmbryo embryo;

        public Pawn mother;

        public Pawn father;

        public GeneSet geneSet;

        public XenotypeDef xenotypeDef;

        public CompTemperatureRuinable tempComp;

        public float progressSpeed = 1f;

        public bool hatched = false;

        public CompProperties_HumanoidHatcher Props => (CompProperties_HumanoidHatcher)props;

        public bool TemperatureDamaged
        {
            get
            {
                if (tempComp != null)
                {
                    return tempComp.Ruined;
                }
                return false;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref gestateProgress, "gestateProgress");
            Scribe_References.Look(ref mother, "mother");
            Scribe_References.Look(ref father, "father");
            Scribe_Deep.Look(ref geneSet, "geneSet");
            Scribe_Values.Look(ref hatched, "hatched");
            Scribe_Values.Look(ref progressSpeed, "progressSpeed");
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (tempComp == null)
            {
                tempComp = parent.TryGetComp<CompTemperatureRuinable>();
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (xenotypeDef == null)
            {
                xenotypeDef = mother?.genes?.Xenotype ?? XenotypeDefOf.Baseliner;
            }
            /*if (hatchee == null)
            {
                // Generate child ASAP so it doesn't magically have differences made to the parents during the time the egg exists.
                GenerateChild();
            }*/
            if(embryo == null)
            {
                //using vanilla fertilization mechanics to avoid using the internals that got changed to private in the update
                HumanOvum ovum = ThingMaker.MakeThing(ThingDefOf.HumanOvum) as HumanOvum;
                ovum.TryGetComp<CompHasPawnSources>().AddSource(mother);
                embryo = ovum.ProduceEmbryo(father) as HumanEmbryo;
            }
            if (!TemperatureDamaged)
            {
                float num = (1f / (Props.daysToHatch * 60000f)) * progressSpeed;
                gestateProgress += num;
                if (gestateProgress > 1f)
                {
                    Hatch();
                }
            }
            if (hatched)
            {
                embryo.Destroy();
                parent.Destroy();
            }
        }

        public void Hatch()
        {
            Precept_Ritual ritual = Faction.OfPlayer.ideos.PrimaryIdeo.GetPrecept(PreceptDefOf.ChildBirth) as Precept_Ritual;
            Thing thing = PregnancyUtility.ApplyBirthOutcome(RitualOutcomeEffectDefOf.ChildBirth.BestOutcome, 1f, ritual, embryo?.GeneSet?.GenesListForReading, embryo.Mother, parent, embryo.Father, preventLetter: true);
            if(thing != null) {     
                thing.SetFaction(Faction.OfPlayer);
                // Send Letter
                SendLetter(thing);
                hatched = true;
            }
        }

        public void SendLetter(Thing child)
        {
            if (mother == null)
            {
                Log.Warning("Mother is null, this shouldn't happen but here we are I guess.");
                return;
            }
            ChoiceLetter_BabyBirth choiceLetter_BabyBirth = (ChoiceLetter_BabyBirth)LetterMaker.MakeLetter("Outland.EggHatchedLabel".Translate(mother.NameShortColored), "Outland.EggHatchedDesc".Translate(mother.NameShortColored), LetterDefOf.BabyBirth, (TargetInfo)child);
            choiceLetter_BabyBirth.Start();
            Find.LetterStack.ReceiveLetter(choiceLetter_BabyBirth);
        }

        public override bool AllowStackWith(Thing other)
        {
            return false;
        }

        public override string CompInspectStringExtra()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(base.CompInspectStringExtra());
            if (mother != null)
            {
                builder.AppendLine($"Mother: {mother}");
            }
            builder.AppendLine("Progress: " + gestateProgress.ToStringPercent());
            builder.Append("Time Left: " + ((int)((1f - gestateProgress) * (Props.daysToHatch / progressSpeed) * 60000f)).ToStringTicksToPeriod());
            return builder.ToString();
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            // QOL: Look into making the genes screen able to be shown for eggs.
            // Currently just shows a blank screen if forced.
            //if (geneSet != null)
            //{

            //    yield return new Command_Action
            //    {
            //        defaultLabel = "InspectBabyGenes".Translate() + "...",
            //        defaultDesc = "InspectGenesHediffDesc".Translate(),
            //        icon = GeneSetHolderBase.GeneticInfoTex.Texture,
            //        action = delegate
            //        {
            //            InspectPaneUtility.OpenTab(typeof(ITab_GenesEgg));
            //        }
            //    };
            //}
            if (!DebugSettings.ShowDevGizmos)
            {
                yield break;
            }
            yield return new Command_Action()
            {
                defaultLabel = "DEV: Hatch Now",
                action = delegate
                {
                    Hatch();
                }
            };
        }
    }
}
