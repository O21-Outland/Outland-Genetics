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
    [HarmonyPatch(typeof(GeneDefGenerator), "ImpliedGeneDefs")]
    public static class Patch_GeneDefGenerator_ImpliedGeneDefs
    {
        [HarmonyPostfix]
        public static IEnumerable<GeneDef> Postfix(IEnumerable<GeneDef> values)
        {
            List<GeneDef> results = values.ToList();
            foreach(AscensionGeneTemplateDef def in DefDatabase<AscensionGeneTemplateDef>.AllDefs)
            {
                List<XenotypeDef> blacklist = new List<XenotypeDef> { XenotypeDefOf.Baseliner };
                foreach(XenotypeDef xenoDef in DefDatabase<XenotypeDef>.AllDefs)
                {
                    if (blacklist.Contains(xenoDef))
                    {
                        continue;
                    }
                    DefModExt_GeneSpecifics modExt = xenoDef.GetModExtension<DefModExt_GeneSpecifics>();
                    if (modExt != null && modExt.xenoAscensionBlacklist)
                    {
                        continue;
                    }
                    results.Add(CreateXenoTypeAscensionDef(def, xenoDef, xenoDef.index * 1000));
                }
            }
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
                    results.Add(CreateXenoTypeImplanterDef(def, xenoDef, xenoDef.index * 1000));
                }
            }
            foreach (XenoReproductionGeneTemplateDef def in DefDatabase<XenoReproductionGeneTemplateDef>.AllDefs)
            {
                List<XenotypeDef> blacklist = new List<XenotypeDef> { };
                foreach (XenotypeDef xenoDef in DefDatabase<XenotypeDef>.AllDefs)
                {
                    if (blacklist.Contains(xenoDef))
                    {
                        continue;
                    }
                    DefModExt_GeneSpecifics modExt = xenoDef.GetModExtension<DefModExt_GeneSpecifics>();
                    if (modExt != null && modExt.xenoEggBlacklist)
                    {
                        continue;
                    }
                    results.Add(CreateXenoReproductionGeneDef(def, xenoDef, xenoDef.index * 1000));
                }
            }
            return results;
        }

        public static GeneDef CreateXenoTypeImplanterDef(XenoImplanterGeneTemplateDef template, XenotypeDef xenotype, int displayOrder)
        {
            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + xenotype.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(xenotype.label),
                iconPath = xenotype.iconPath,
                description = template.description.Formatted(xenotype.LabelCap),
                labelShortAdj = template.labelShortAdj.Formatted(xenotype.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrder + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = template.modExtensions,
                canGenerateInGeneSet = false
            };
            geneDef.modExtensions.Add(new DefModExt_Xenotype() { xenotypeDef = xenotype });
            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }
            geneDef.abilities = new List<AbilityDef>();
            geneDef.abilities.Add(DefDatabase<AbilityDef>.GetNamed(template.defName + "_" + xenotype.defName));
            return geneDef;
        }

        public static GeneDef CreateXenoTypeAscensionDef(AscensionGeneTemplateDef template, XenotypeDef xenotype, int displayOrder)
        {
            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + xenotype.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(xenotype.label),
                iconPath = xenotype.iconPath,
                description = template.description.Formatted(xenotype.LabelCap),
                labelShortAdj = template.labelShortAdj.Formatted(xenotype.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrder + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = template.modExtensions,
                canGenerateInGeneSet = false,
                customEffectDescriptions = template.customEffectDescriptions,
            };
            geneDef.modExtensions.Add(new DefModExt_Xenotype() { xenotypeDef = xenotype });
            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }
            return geneDef;
        }

        public static GeneDef CreateXenoReproductionGeneDef(XenoReproductionGeneTemplateDef template, XenotypeDef xenotype, int displayOrder)
        {
            GeneDef geneDef = new GeneDef
            {
                defName = template.defName + "_" + xenotype.defName,
                geneClass = template.geneClass,
                label = template.label.Formatted(xenotype.label),
                iconPath = xenotype.iconPath,
                description = template.description.Formatted(xenotype.LabelCap),
                labelShortAdj = template.labelShortAdj.Formatted(xenotype.label),
                selectionWeight = template.selectionWeight,
                biostatCpx = template.biostatCpx,
                biostatMet = template.biostatMet,
                displayCategory = template.displayCategory,
                displayOrderInCategory = displayOrder + template.displayOrderOffset,
                minAgeActive = template.minAgeActive,
                modContentPack = template.modContentPack,
                modExtensions = template.modExtensions,
                canGenerateInGeneSet = false,
                customEffectDescriptions = template.customEffectDescriptions,
            };
            geneDef.modExtensions.Add(new DefModExt_Xenotype() { xenotypeDef = xenotype });
            if (!template.exclusionTagPrefix.NullOrEmpty())
            {
                geneDef.exclusionTags = new List<string> { template.exclusionTagPrefix };
            }
            return geneDef;
        }
    }
}
