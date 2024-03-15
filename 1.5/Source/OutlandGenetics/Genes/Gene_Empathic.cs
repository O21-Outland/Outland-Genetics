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
    public class Gene_Empathic : Gene
    {
        public int nextMoodTick = -1;

        public List<Thought> tmpTotalMoodOffsetThoughts = new List<Thought>();

        public IEnumerable<Pawn> ValidPawns
        {
            get
            {
                if (!pawn.Spawned) { yield break; }
                foreach(Pawn p in pawn.Map.mapPawns.FreeAdultColonistsSpawned)
                {
                    if (p != pawn && p.relations != null)
                    {
                        yield return p;
                    }
                }
                yield break;
            }
        }

        public int GetColonyMood
        {
            get
            {
                float totalMood = 0f;
                foreach (Pawn colPawn in ValidPawns)
                {
                    float colPawnMood = TotalMoodOffset(colPawn);
                    if (pawn.relations?.OpinionOf(colPawn) >= 50)
                    {
                        totalMood += (colPawnMood * 2f);
                    }
                    else
                    {
                        totalMood += colPawnMood;
                    }
                }
                return Mathf.RoundToInt(totalMood * 0.1f);
            }
        }

        public float TotalMoodOffset(Pawn pawn)
        {
            pawn?.needs?.mood?.thoughts?.GetDistinctMoodThoughtGroups(tmpTotalMoodOffsetThoughts);
            float num = 0f;
            if (tmpTotalMoodOffsetThoughts.NullOrEmpty())
            {
                return 0f;
            }
            for (int i = 0; i < tmpTotalMoodOffsetThoughts.Count; i++)
            {
                if(tmpTotalMoodOffsetThoughts[i].def != OutlandGenesDefOf.Outland_Empathic)
                {
                    num += pawn.needs.mood.thoughts.MoodOffsetOfGroup(tmpTotalMoodOffsetThoughts[i]);
                }
            }
            tmpTotalMoodOffsetThoughts.Clear();
            return num;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref nextMoodTick, "nextMoodTick", -1);
        }

        public override void Tick()
        {
            base.Tick();
            if (!pawn.Spawned) { return; }
            if (nextMoodTick == -1)
            {
                nextMoodTick = Find.TickManager.TicksGame + 300;
            }
            if (nextMoodTick < Find.TickManager.TicksGame)
            {
                UpdateEmpathy();
            }
        }

        public void UpdateEmpathy()
        {
            Thought_Memory oldMemory = pawn.needs.mood.thoughts.memories.GetFirstMemoryOfDef(OutlandGenesDefOf.Outland_Empathic);
            int moodOffset = ValidPawns.EnumerableNullOrEmpty() ? 0 : GetColonyMood;
            if (oldMemory == null && moodOffset != 0)
            {
                Thought_Memory memory = new Thought_Memory
                {
                    def = OutlandGenesDefOf.Outland_Empathic,
                    moodOffset = moodOffset
                };
                pawn.needs.mood.thoughts.memories.TryGainMemory(memory);
            }
            else
            {
                if(moodOffset != 0)
                {
                    oldMemory.moodOffset = moodOffset;
                    oldMemory.Renew();
                }
            }
            nextMoodTick = Find.TickManager.TicksGame + 300;
        }
    }
}
