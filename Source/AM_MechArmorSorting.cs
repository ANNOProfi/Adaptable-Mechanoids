using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace AdaptableMechanoids
{
    public class AM_MechArmorSorting : IExposable
    {
        private Dictionary<FactionDef, AM_MechArmorStats> mechs = new Dictionary<FactionDef, AM_MechArmorStats>();

        public AM_MechArmorSorting()
        {

        }

        public void MakeNewMech(Pawn pawn, AM_AdaptableArmor armorTypes, FactionDef mechFaction)
        {
            if(mechs.ContainsKey(mechFaction))
            {
                Log.Warning("AM_Error: Could not make new mech "+ pawn.def.defName +", already registered");
            }
            else
            {
                mechs.Add(mechFaction, new AM_MechArmorStats());
                mechs[mechFaction].Register(pawn, armorTypes);
            }
        }

        public AM_MechArmorStats CheckMech(FactionDef mechFaction)
        {
            foreach(FactionDef mechFactions in mechs.Keys)
            {
                if(mechFactions == mechFaction)
                {
                    return mechs[mechFaction];
                }
            }

            return null;
        }

        public void AdaptMechs(bool hardmode)
        {
            foreach(FactionDef mechFactions in mechs.Keys)
            {
                mechs[mechFactions].CalculateArmor(hardmode);
            }
        }

        public void AddDamage(FactionDef mechFaction, DamageArmorCategoryDef damage, float damageAmount)
        {
            if(mechs.ContainsKey(mechFaction))
            {
                mechs[mechFaction].damageAmounts[damage] += damageAmount;
                mechs[mechFaction].CalculateNewArmor();
            }
            else
            {
                Log.Warning("AM_Warning: Mech tried to add damage in sorting before being registered");
            }
        }

        public void ResetArmor(bool debug)
        {
            foreach(FactionDef mechFactions in mechs.Keys)
            {
                if(debug)
                {
                    Log.Message("AM_Debug: Resetting armor for faction "+mechFactions.defName);
                }
                mechs[mechFactions].ResetArmor(debug);
            }
        }

        public void ResetMax()
        {
            foreach(FactionDef mechFactions in mechs.Keys)
            {
                mechs[mechFactions].ResetMax();
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref mechs, "mechs", LookMode.Def, LookMode.Deep);
        }

        /*public string GetUniqueLoadID()
        {
            return "AM_MechArmorSorting_"+loadID;
        }*/
    }
}