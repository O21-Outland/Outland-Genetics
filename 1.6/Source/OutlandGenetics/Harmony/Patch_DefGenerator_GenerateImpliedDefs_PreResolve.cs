using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace OutlandGenes
{
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    public static class Patch_DefGenerator_GenerateImpliedDefs_PreResolve
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            foreach (AbilityDef item in ImpliedAbilityDefs())
            {
                DefGenerator.AddImpliedDef(item);
            }
        }

        public static IEnumerable<AbilityDef> ImpliedAbilityDefs()
        {
            foreach (XenoImplanterGeneTemplateDef def in DefDatabase<XenoImplanterGeneTemplateDef>.AllDefs)
            {
                List<XenotypeDef> blacklist = new List<XenotypeDef> { XenotypeDefOf.Baseliner };
                foreach (XenotypeDef xenoDef in DefDatabase<XenotypeDef>.AllDefs)
                {
                    if (blacklist.Contains(xenoDef))
                    {
                        continue;
                    }
                    DefModExt_GeneSpecifics modExt = xenoDef.GetModExtension<DefModExt_GeneSpecifics>();
                    if (modExt != null && modExt.xenoImplanterBlacklist)
                    {
                        continue;
                    }
                    yield return CreateXenoTypeImplanterAbility(def, xenoDef, xenoDef.index * 1000);
                }
            }
        }

        public static AbilityDef CreateXenoTypeImplanterAbility(XenoImplanterGeneTemplateDef template, XenotypeDef xenotype, int displayOrder)
        {
            AbilityDef ability = new AbilityDef
            {
                defName = template.defName + "_" + xenotype.defName,
                label = template.label.Formatted(xenotype.label),
                iconPath = xenotype.iconPath,
                description = template.description.Formatted(xenotype.LabelCap),
                stunTargetWhileCasting = true,
                displayGizmoWhileUndrafted = true,
                disableGizmoWhileUndrafted = false,
                jobDef = OutlandGenesDefOf.CastAbilityOnThingMelee,
                warmupStartSound = OutlandGenesDefOf.ReimplantXenogerm_Cast,
                warmupEffecter = OutlandGenesDefOf.Implant_Xenogerm,
                hostile = false,
                displayOrder = 500,
                cooldownTicksRange = new IntRange(600000, 600000),
                verbProperties = new VerbProperties
                {
                    verbClass = typeof(Verb_CastAbilityTouch),
                    drawAimPie = false,
                    range = -1f,
                    warmupTime = 4f,
                    targetParams = new TargetingParameters
                    {
                        canTargetAnimals = false,
                        canTargetSelf = false,
                        canTargetBuildings = false,
                        canTargetMechs = false
                    }
                },
                comps = new List<AbilityCompProperties>()
                {
                    new CompProperties_XenotypeImplanter() { xenotype = xenotype }
                }
            };
            return ability;
        }
    }
}
