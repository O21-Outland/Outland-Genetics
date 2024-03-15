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
    public class Hediff_XenotypeAscender : HediffWithComps
    {
        public float currentExp = 0f;

        public bool CanAscend => currentExp >= 1f;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref currentExp, "currentExp", 0f);
            Scribe_Defs.Look(ref targetXenotype, "targetXenotype");
        }

        public XenotypeDef targetXenotype = null;

        public void GiveExp(float exp)
        {
            if (currentExp + exp > 1f)
            {
                currentExp = 1f;
            }
            else
            {
                currentExp += exp;
            }
        }

        public void Ascend()
        {
            List<Gene> list3 = pawn.genes.Endogenes;
            for (int num = list3.Count - 1; num >= 0; num--)
            {
                Gene gene2 = list3[num];
                if (gene2.def.endogeneCategory != EndogeneCategory.Melanin && gene2.def.endogeneCategory != EndogeneCategory.HairColor)
                {
                    pawn.genes.RemoveGene(gene2);
                }
            }
            pawn.genes.SetXenotype(targetXenotype);

            pawn.health.RemoveHediff(this);
        }

        public bool HasAllRequiredSkills(List<SkillRange> skillRanges)
        {
            foreach (SkillRange skillRange in skillRanges)
            {
                if (!skillRange.Range.Includes(pawn.skills.GetSkill(skillRange.Skill).Level))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasAnyRequiredTrait(List<TraitDef> traits)
        {
            foreach (TraitDef trait in traits)
            {
                if (pawn.story.traits.HasTrait(trait))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
