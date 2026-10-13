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
    public class Thought_KindEyes : Thought_SituationalSocial
	{
		public override float OpinionOffset()
		{
			return OtherPawn().genes.HasGene(OutlandGenesDefOf.Outland_KindEyes) ? 20 : 0;
		}
	}
}
