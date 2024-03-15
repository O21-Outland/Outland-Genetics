using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace OutlandGenes
{
    public class JobDriver_FrozenInIce : JobDriver
	{
		public override string GetReport()
		{
			return "ReportStanding".Translate();
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}

		public override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil
			{
				initAction = delegate
				{
					base.Map.pawnDestinationReservationManager.Reserve(pawn, job, pawn.Position);
					pawn.pather.StopDead();
				}
			};
			DecorateWaitToil(toil);
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			if (job.overrideFacing != Rot4.Invalid)
			{
				toil.handlingFacing = true;
				toil.tickAction = (Action)Delegate.Combine(toil.tickAction, (Action)delegate
				{
					pawn.rotationTracker.FaceTarget(pawn.Position + job.overrideFacing.FacingCell);
				});
			}
			yield return toil;
		}

		public virtual void DecorateWaitToil(Toil wait) { }

		public override void Notify_StanceChanged()
		{
			if (pawn.stances.curStance is Stance_Mobile)
			{
				EndJobWith(JobCondition.InterruptOptional);
			}
		}
	}
}
