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

        public Pawn hatchee;

        public Pawn mother;

        public Pawn father;

        public GeneSet geneSet;

        public XenotypeDef xenotype;

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
            Scribe_Deep.Look(ref hatchee, "hatchee");
            Scribe_References.Look(ref mother, "mother");
            Scribe_References.Look(ref father, "father");
            Scribe_Deep.Look(ref geneSet, "geneSet");
            Scribe_Values.Look(ref hatched, "hatched");
            Scribe_Values.Look(ref progressSpeed, "progressSpeed");
        }

        public void GenerateChild()
        {
            PawnGenerationRequest request = new PawnGenerationRequest(mother?.kindDef ?? PawnKindDefOf.Colonist, Faction.OfPlayer, PawnGenerationContext.NonPlayer, -1, forceGenerateNewPawn: false, allowDead: false, allowDowned: true, canGeneratePawnRelations: true, mustBeCapableOfViolence: false, 1f, forceAddFreeWarmLayerIfNeeded: false, allowGay: true, allowPregnant: false, allowFood: true, allowAddictions: true, inhabitant: false, certainlyBeenInCryptosleep: false, forceRedressWorldPawnIfFormerColonist: false, worldPawnFactionDoesntMatter: false, 0f, 0f, null, 1f, null, null, null, null, null, null, null, null, PregnancyUtility.RandomLastName(mother, null, father), null, null, null, forceNoIdeo: true, forceNoBackstory: false, forbidAnyTitle: false, forcedXenotype: xenotype, forcedEndogenes: (geneSet != null) ? geneSet.genes : PregnancyUtility.GetInheritedGenes(father, mother), forcedCustomXenotype: null, allowedXenotypes: null, forceBaselinerChance: 0f, developmentalStages: DevelopmentalStage.Newborn);
            hatchee = PawnGenerator.GeneratePawn(request);
            if (mother != null && father != null)
            {
                if (GeneUtility.SameHeritableXenotype(mother, father) && mother.genes.UniqueXenotype)
                {
                    hatchee.genes.xenotypeName = mother.genes.xenotypeName;
                    hatchee.genes.iconDef = mother.genes.iconDef;
                }
                //hatchee.genes.SetXenotypeDirect(xenotype);
            }
            else if (PregnancyUtility.TryGetInheritedXenotype(mother, father, out var xenotype))
            {
                hatchee.genes?.SetXenotypeDirect(xenotype);
            }
            else if (PregnancyUtility.ShouldByHybrid(mother, father))
            {
                hatchee.genes.hybrid = true;
                hatchee.genes.xenotypeName = "Hybrid".Translate();
            }

            if (mother != null)
            {
                hatchee.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
            }
            if (father != null)
            {
                hatchee.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
            }
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
            if (xenotype == null)
            {
                xenotype = mother?.genes?.Xenotype ?? XenotypeDefOf.Baseliner;
            }
            if (hatchee == null)
            {
                // Generate child ASAP so it doesn't magically have differences made to the parents during the time the egg exists.
                GenerateChild();
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
                parent.Destroy();
            }
        }

        public void Hatch()
        {
            // Reset child and spawn as if newborn.
            hatchee.ageTracker.ageBiologicalTicksInt = 0;
            hatchee.ageTracker.birthAbsTicksInt = Find.TickManager.TicksAbs;
            hatchee.factionInt = Faction.OfPlayer;
            if (PawnUtility.TrySpawnHatchedOrBornPawn(hatchee, parent))
            {
                if (mother != null)
                {
                    if (hatchee.playerSettings != null && mother.playerSettings != null)
                    {
                        hatchee.playerSettings.allowedAreas = mother.playerSettings.allowedAreas;
                    }
                    if (hatchee.RaceProps.IsFlesh)
                    {
                        hatchee.relations.AddDirectRelation(PawnRelationDefOf.Parent, mother);
                        if (father != null)
                        {
                            hatchee.relations.AddDirectRelation(PawnRelationDefOf.Parent, father);
                        }
                    }
                    if (mother.Spawned)
                    {
                        mother.GetLord()?.AddPawn(hatchee);
                    }
                }
                // Send Letter
                SendLetter();
                hatched = true;
            }
        }

        public void SendLetter()
        {
            if (mother == null)
            {
                Log.Warning("Mother is null, this shouldn't happen but here we are I guess.");
                return;
            }
            ChoiceLetter_BabyBirth choiceLetter_BabyBirth = (ChoiceLetter_BabyBirth)LetterMaker.MakeLetter("Outland.EggHatchedLabel".Translate(mother.NameShortColored), "Outland.EggHatchedDesc".Translate(mother.NameShortColored), LetterDefOf.BabyBirth, (TargetInfo)hatchee);
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
            yield return new Command_Action()
            {
                defaultLabel = "DEV: Regenerate Child",
                action = delegate
                {
                    RegenerateChild();
                }
            };
        }

        public void RegenerateChild()
        {
            hatchee.Discard();
            GenerateChild();
        }
    }
}
