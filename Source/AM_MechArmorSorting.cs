using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace AdaptableMechanoids
{
    public class AM_MechArmorSorting : IExposable
    {
        private Dictionary<AM_MechKinds, AM_MechArmorStats> mechs = new Dictionary<AM_MechKinds, AM_MechArmorStats>();

        public AM_MechArmorSorting()
        {

        }

        public void MakeNewMech(Pawn pawn, AM_AdaptableArmor armorTypes, AM_MechKinds mechKind)
        {
            if(mechs.ContainsKey(mechKind))
            {
                Log.Warning("AM_Error: Could not make new mech "+ pawn.def.defName +", already registered");
            }
            else
            {
                mechs.Add(mechKind, new AM_MechArmorStats());
                mechs[mechKind].Register(pawn, armorTypes);
            }
        }

        public AM_MechArmorStats CheckMech(AM_MechKinds mechKind)
        {
            foreach(AM_MechKinds mechKinds in mechs.Keys)
            {
                if(mechKinds == mechKind)
                {
                    return mechs[mechKind];
                }
            }

            return null;
        }

        public void AdaptMechs(bool hardmode)
        {
            foreach(AM_MechKinds mechKinds in mechs.Keys)
            {
                mechs[mechKinds].CalculateArmor(hardmode);
            }
        }

        public void AddDamage(AM_MechKinds mechKind, DamageArmorCategoryDef damage, float damageAmount)
        {
            if(mechs.ContainsKey(mechKind))
            {
                mechs[mechKind].damageAmounts[damage] += damageAmount;
            }
            else
            {
                Log.Warning("AM_Warning: Mech tried to add damage in sorting before being registered");
            }
        }

        public void ResetArmor(bool debug)
        {
            foreach(AM_MechKinds mechKinds in mechs.Keys)
            {
                if(debug)
                {
                    Log.Message("AM_Debug: Resetting armor for mechKind "+mechKinds);
                }
                mechs[mechKinds].ResetArmor(debug);
            }
        }

        public void ResetMax()
        {
            foreach(AM_MechKinds mechKinds in mechs.Keys)
            {
                mechs[mechKinds].ResetMax();
            }
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref mechs, "mechs", LookMode.Value, LookMode.Deep);
        }

        /*public string GetUniqueLoadID()
        {
            return "AM_MechArmorSorting_"+loadID;
        }*/
    }
}