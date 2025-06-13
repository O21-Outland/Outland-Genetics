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
    public static class PreggoUtil
	{
		public static List<GeneDef> tmpGenes = new List<GeneDef>();

		public static List<GeneDef> tmpGenesShuffled = new List<GeneDef>();

		private static Dictionary<GeneDef, float> tmpGeneChances = new Dictionary<GeneDef, float>();

		public static List<GeneDef> GetInheritedGenes(Pawn inheritance, out bool success)
		{
			tmpGenes.Clear();
			tmpGenesShuffled.Clear();
			if (inheritance?.genes != null)
			{
				foreach (Gene endogene in inheritance.genes.Endogenes)
				{
					if (endogene.def.endogeneCategory != EndogeneCategory.Melanin && endogene.def.biostatArc <= 0)
					{
						if (!tmpGenesShuffled.Contains(endogene.def))
						{
							tmpGenesShuffled.Add(endogene.def);
						}
					}
				}
			}
			int num = 0;
			do
			{
				tmpGenes.Clear();
				tmpGenesShuffled.Shuffle();
				foreach (GeneDef item in tmpGenesShuffled)
				{
					if (!tmpGenes.Contains(item))
					{
						tmpGenes.Add(item);
					}
				}
				tmpGenes.RemoveAll((GeneDef x) => x.prerequisite != null && !tmpGenes.Contains(x.prerequisite));
				int val = tmpGenes.NonOverriddenGenes(xenogene: false).Sum((GeneDef x) => x.biostatMet);
				if (GeneTuning.BiostatRange.Includes(val))
				{
					break;
				}
				num++;
			}
			while (num < 100);
			success = num < 100;
			if (PawnSkinColors.SkinColorsFromParents(inheritance, inheritance).TryRandomElement(out var result))
			{
				tmpGenes.Add(result);
			}
			if (!tmpGenes.Any((GeneDef x) => x.endogeneCategory == EndogeneCategory.HairColor))
			{
				GeneDef geneDef = inheritance?.genes?.GetFirstEndogeneByCategory(EndogeneCategory.HairColor);
				GeneDef result2;
				if (geneDef != null)
				{
					tmpGenes.Add(geneDef);
				}
				else if (DefDatabase<GeneDef>.AllDefs.Where((GeneDef x) => x.endogeneCategory == EndogeneCategory.HairColor).TryRandomElementByWeight((GeneDef x) => x.selectionWeight, out result2))
				{
					tmpGenes.Add(result2);
				}
			}
			tmpGeneChances.Clear();
			tmpGenesShuffled.Clear();
			return tmpGenes;
		}

		public static GeneSet GetInheritedGeneSet(Pawn mother, out bool success)
		{
			GeneSet geneSet = new GeneSet();
			foreach (GeneDef inheritedGene in GetInheritedGenes(mother, out success))
			{
				geneSet.AddGene(inheritedGene);
			}
			geneSet.SetNameDirect(mother.genes.xenotypeName);
			return geneSet;
		}

		public static GeneSet GetInheritedGeneSet(Pawn mother)
		{
			GeneSet geneSet = new GeneSet();
			bool success;
			foreach (GeneDef inheritedGene in GetInheritedGenes(mother, out success))
			{
				geneSet.AddGene(inheritedGene);
			}
			geneSet.SetNameDirect(mother.genes.xenotypeName);
			return geneSet;
		}
	}
}
