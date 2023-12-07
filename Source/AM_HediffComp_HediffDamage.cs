using System;
using RimWorld;
using Verse;

namespace AdaptableMechanoids
{
    public class AM_HediffComp_HediffDamage : HediffComp
    {
        private AM_GameComponent_Adaptation component = Current.Game.GetComponent<AM_GameComponent_Adaptation>();

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            if(!this.Pawn.Dead && (this.Pawn.RaceProps.IsMechanoid && !this.Pawn.IsColonyMech))
            {
                Log.Message("Trying to upload "+totalDamageDealt + " "+ dinfo.Def.armorCategory+" damage");
                component.mechList[Pawn.def.defName].damageAmounts[dinfo.Def.armorCategory] += totalDamageDealt;
                Log.Message("Uploaded "+component.mechList[Pawn.def.defName].damageAmounts[dinfo.Def.armorCategory]+" "+dinfo.Def.armorCategory +" damage to "+Pawn.def.defName);
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);

            if(!component.mechList.ContainsKey(this.Pawn.def.defName))
            {
                Log.Message("Generating new entry");
                component.mechList.Add(this.Pawn.def.defName, new AM_MechArmorStats(Pawn.GetStatValue(StatDefOf.ArmorRating_Blunt), Pawn.GetStatValue(StatDefOf.ArmorRating_Sharp), Pawn.GetStatValue(StatDefOf.ArmorRating_Heat), Pawn.def.defName));
            }
        }
    }
}