using System.Collections.Generic;
using Verse;

namespace AdaptableMechanoids
{
    internal class AM_HediffGiver_Armor : HediffGiver
    {
        public override void OnIntervalPassed(Pawn pawn, Hediff cause)
        {
            if(!pawn.health.hediffSet.HasHediff(hediff))
            {
                if((AM_Utilities.Settings.adaptAIMech && !pawn.IsColonyMech && !pawn.Faction.def.humanlikeFaction) || (AM_Utilities.Settings.adaptColonyMech && pawn.IsColonyMech))
                {
                    HediffGiverUtility.TryApply(pawn, hediff, (IEnumerable<BodyPartDef>)null, false, 1, (List<Hediff>)null);
                }
            }
        }
    }
}