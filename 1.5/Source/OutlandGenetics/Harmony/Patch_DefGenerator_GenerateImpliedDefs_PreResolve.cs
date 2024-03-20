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

            foreach (ThingDef item in ImpliedThingDefs())
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

        public static IEnumerable<ThingDef> ImpliedThingDefs()
        {
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
                    if(modExt != null && modExt.xenoEggBlacklist)
                    {
                        continue;
                    }
                    yield return CreateXenotypeEgg(def, xenoDef, xenoDef.index * 1000);
                }
            }
        }

        public static ThingDef CreateXenotypeEgg(XenoReproductionGeneTemplateDef template, XenotypeDef xenotype, int displayOrder)
        {
            DefModExt_GeneSpecifics modExt = xenotype.GetModExtension<DefModExt_GeneSpecifics>();
            ThingDef thing = new ThingDef
            {
                defName = xenotype.defName + "_XenoEgg",
                label = "Outland.XenotypeEggLabel".Translate(xenotype.label).Formatted(),
                description = "Outland.XenotypeEggDesc".Translate(xenotype.label).Formatted(),
                thingClass = typeof(ThingWithComps),
                graphicData = new GraphicData()
                {
                    texPath = modExt?.xenoEggTexturePath ?? "Outland/Items/XenoEgg_Baseliner",
                    shaderType = ShaderTypeDefOf.Cutout,
                    graphicClass = typeof(Graphic_Single)
                },
                category = ThingCategory.Item,
                drawerType = DrawerType.MapMeshOnly,
                resourceReadoutPriority = ResourceCountPriority.Middle,
                useHitPoints = true,
                selectable = true,
                tradeability = Tradeability.None,
                altitudeLayer = AltitudeLayer.Item,
                stackLimit = 1,
                tickerType = TickerType.Normal,
                thingCategories = new List<ThingCategoryDef>() { OutlandGenesDefOf.Outland_HumanoidEggs },
                statBases = new List<StatModifier>()
                {
                    new StatModifier() { stat = StatDefOf.Beauty, value = -4 },
                    new StatModifier() { stat = StatDefOf.Mass, value = 0.3f },
                    new StatModifier() { stat = StatDefOf.Flammability, value = 0.7f },
                    new StatModifier() { stat = StatDefOf.DeteriorationRate, value = 2 },
                    new StatModifier() { stat = StatDefOf.MarketValue, value = 300 }
                },
                comps = new List<CompProperties>()
                {
                    new CompProperties_Forbiddable(),
                    new CompProperties_TemperatureRuinable()
                    {
                        minSafeTemperature = 0,
                        maxSafeTemperature = 50,
                        progressPerDegreePerTick = 0.00003f
                    },
                    new CompProperties_XenotypeHatcher()
                    {
                        hatcherPawn = PawnKindDefOf.Colonist,
                        hatcherXenotype = xenotype,
                        hatcherDaystoHatch = 14
                    }
                },
                alwaysHaulable = true,
                drawGUIOverlay = true,
                rotatable = false,
                pathCost = 14,
                allowedArchonexusCount = 1
            };
            return thing;
        }
    }
}
