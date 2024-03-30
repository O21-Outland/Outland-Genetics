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
    public class AscensionGeneTemplateDef : Def
	{
		public Type geneClass = typeof(Gene);

		public int biostatCpx;

		public int biostatMet;

		public float minAgeActive;

		public GeneCategoryDef displayCategory;

		public int displayOrderOffset;

		public float selectionWeight = 1f;

		public List<string> customEffectDescriptions = new List<string>();

		[MustTranslate]
		public string labelShortAdj;

		[NoTranslate]
		public string iconPath;

		[NoTranslate]
		public string exclusionTagPrefix;

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
			if (!typeof(Gene).IsAssignableFrom(geneClass))
			{
				yield return "geneClass is not Gene or child thereof.";
			}
		}
	}
}
