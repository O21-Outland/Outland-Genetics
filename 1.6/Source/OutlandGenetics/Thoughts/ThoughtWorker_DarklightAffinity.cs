using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace OutlandGenes
{
    public class ThoughtWorker_DarklightAffinity : ThoughtWorker
	{
		public override ThoughtState CurrentStateInternal(Pawn p)
		{
			if(p.Awake() && p.IsColonist && !PawnUtility.IsBiologicallyOrArtificiallyBlind(p))
			{
				if (p.genes.HasGene(OutlandGenesDefOf.Outland_DarklightAffinity))
				{
					if (!DarklightUtility.IsDarklight(p.Map.glowGrid.VisualGlowAt(p.Position)))
					{
						float Brightness = BrightnessAt(p.Position, p.Map);
						if (Brightness > 0.9f)
						{
							return ThoughtState.ActiveAtStage(1);
						}
						if (Brightness > 0.45f)
						{
							return ThoughtState.ActiveAtStage(0);
						}
						return ThoughtState.Inactive;
					}
				}
			}
			return false;
		}

		public float BrightnessAt(IntVec3 position, Map map)
		{
			if (position.InBounds(map))
			{
				return map.glowGrid.GroundGlowAt(position);
			}
			return 0;
		}
	}
}
