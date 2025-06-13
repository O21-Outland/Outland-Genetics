using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OutlandGenes
{
    public class JobDriver_LayEgg : JobDriver
    {
        public const int LayEgg = 500;

        public Thing Target => TargetA.Thing;

        public Hediff_EggPouch EggPouch => (Hediff_EggPouch)pawn.health.hediffSet.GetFirstHediffOfDef(OutlandGenesDefOf.Outland_EggPouch);

        public CompEggContainer EggBoxComp => job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompEggContainer>();

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.CanReserveAndReach(Target, PathEndMode.OnCell, Danger.Deadly))
            {
                pawn.Reserve(Target, job, errorOnFailed: errorOnFailed);
                return true;
            }

            return false;
        }

        public override RandomSocialMode DesiredSocialMode()
        {
            return RandomSocialMode.Off;
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell);
            yield return Toils_General.Wait(500);
            yield return Toils_General.Do(delegate
            {
                Thing xenoEgg = ThingMaker.MakeThing(EggPouch.curEggDef);
                Comp_XenotypeHatcher comp = xenoEgg.TryGetComp<Comp_XenotypeHatcher>();
                comp.hatcheeParent = pawn;
                comp.progressSpeed = GeneUtil.GetPregnancySpeed(pawn);
                if (job.GetTarget(TargetIndex.A).HasThing && EggBoxComp.Accepts(xenoEgg.def))
                {
                    EggBoxComp.innerContainer.TryAdd(xenoEgg);
                }
                else
                {
                    GenPlace.TryPlaceThing(xenoEgg, pawn.Position, pawn.Map, ThingPlaceMode.Near, delegate (Thing t, int i)
                    {
                        if (pawn.Faction != Faction.OfPlayer)
                        {
                            t.SetForbidden(value: true);
                        }
                    });
                }
                pawn.health.AddHediff(OutlandGenesDefOf.Outland_EggFatigue);
            });
        }
    }
}
