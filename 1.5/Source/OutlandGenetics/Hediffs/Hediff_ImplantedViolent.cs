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
    public class Hediff_ImplantedViolent : Hediff_Implanted
    {
        public override void SendLetter(Thing thing)
        {
            ChoiceLetter_BabyBirth choiceLetter_BabyBirth = (ChoiceLetter_BabyBirth)LetterMaker.MakeLetter("OutlandGenes.ParasiteBirthLabel".Translate(), "OutlandGenes.ParasiteBirthDesc".Translate(pawn), LetterDefOf.BabyBirth, thing);
            choiceLetter_BabyBirth.Start();
            Find.LetterStack.ReceiveLetter(choiceLetter_BabyBirth);
        }

        public override void PostRemoved()
        {
            base.PostRemoved();
            pawn.DropAndForbidEverything();
            pawn.apparel.DropAll(pawn.Position);
            pawn.equipment.DropAllEquipment(pawn.Position);
            MakeBlood(pawn.Position, pawn.Map);
            pawn.DeSpawn();
            pawn.Kill(null);
        }

        public void MakeBlood(IntVec3 pos, Map map)
        {
            if (pawn.RaceProps.BloodDef == null)
            {
                return;
            }

            List<IntVec3> list = GenAdjFast.AdjacentCells8Way(pos);

            for (int i = 0; i < list.Count(); i++)
            {
                if (pos.IsValid)
                {
                    FilthMaker.TryMakeFilth(list[i], pawn.Map, pawn.RaceProps.BloodDef);
                }
            }
        }
    }
}
