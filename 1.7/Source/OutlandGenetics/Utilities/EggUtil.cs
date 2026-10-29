using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace OutlandGenes
{
    [StaticConstructorOnStartup]
    public static class EggUtil
	{
		public static Thing GetBestEggBox(Pawn pawn, ThingDef eggDef)
		{
			return GenClosest.ClosestThing_Regionwise_ReachablePrioritized(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.EggBox), PathEndMode.ClosestTouch, TraverseParms.For(pawn, Danger.Some), 30f, IsUsableBox, GetScore, 10);
			float GetScore(Thing thing)
			{
				CompEggContainer compEggContainer = thing.TryGetComp<CompEggContainer>();
				if (compEggContainer == null || compEggContainer.Full)
				{
					return 0f;
				}
				return ((float?)compEggContainer.ContainedThing?.stackCount * 5f) ?? 0.5f;
			}
			bool IsUsableBox(Thing thing)
			{
				if (!thing.Spawned)
				{
					return false;
				}
				if (thing.IsForbidden(pawn) || !pawn.CanReserve(thing) || !pawn.Position.InHorDistOf(thing.Position, 30f))
				{
					return false;
				}
				CompEggContainer compEggContainer2 = thing.TryGetComp<CompEggContainer>();
				if (compEggContainer2 == null || !compEggContainer2.Accepts(eggDef))
				{
					return false;
				}
				return true;
			}
		}
	}
}
