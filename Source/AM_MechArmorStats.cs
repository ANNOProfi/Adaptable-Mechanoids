using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using LudeonTK;

namespace AdaptableMechanoids
{
    public class AM_MechArmorStats
    {
        private float Armor_total
        {
            get
            {
                float armorTotal = 0f;
                foreach(DamageArmorCategoryDef damage in armorTypes)
                {
                    armorTotal += armorValues[damage];
                }

                return armorTotal;
            }
        }

        private float DamageAmountsTotal
        {
            get
            {
                float damageTotal = 0f;
                foreach(DamageArmorCategoryDef damage in armorTypes)
                {
                    damageTotal += damageAmounts[damage];
                }

                return damageTotal;
            }
        }

        private float unspentPoints = 0f;

        private float adaptationStep;

        public List<DamageArmorCategoryDef> armorTypes = new List<DamageArmorCategoryDef>();

        public Dictionary<DamageArmorCategoryDef, float> armorValues = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> armorOffsets = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> damageAmounts = new Dictionary<DamageArmorCategoryDef, float>();

        public Dictionary<DamageArmorCategoryDef, float> armorNewValues = new Dictionary<DamageArmorCategoryDef, float>();

        private string defName = null;

        public string DefName{get => defName;}

        private float maxValue = 2;

        private bool usingHeat = false;

        public AM_MechArmorStats(Pawn pawn, AM_AdaptableArmor armor)
        {
            /*armorTypes.Add(AM_DefOf.Blunt);
            armorTypes.Add(DamageArmorCategoryDefOf.Sharp);

            armorValues.Add(AM_DefOf.Blunt, pawn.GetStatValue(StatDefOf.ArmorRating_Blunt));
            armorValues.Add(DamageArmorCategoryDefOf.Sharp, pawn.GetStatValue(StatDefOf.ArmorRating_Sharp));
            armorValues.Add(AM_DefOf.Heat, pawn.GetStatValue(StatDefOf.ArmorRating_Heat));

            armorOffsets.Add(AM_DefOf.Blunt, 0f);
            armorOffsets.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            armorOffsets.Add(AM_DefOf.Heat, 0f);

            armorNewValues.Add(AM_DefOf.Blunt, 0f);
            armorNewValues.Add(DamageArmorCategoryDefOf.Sharp, 0f);
            armorNewValues.Add(AM_DefOf.Heat, 0f);

            

            damageAmounts.Add(AM_DefOf.Blunt, 0f);
            damageAmounts.Add(DamageArmorCategoryDefOf.Sharp, 0f);

            //Only enables heat armor when enabled in settings
            if(AM_Utilities.Settings.useHeat)
            {
                armorTypes.Add(AM_DefOf.Heat);

                damageAmounts.Add(AM_DefOf.Heat, 0f);

                usingHeat = true;
            }*/

            foreach(AM_ArmorTypes damage in armor.armorTypes)
            {
                //The important one, registers the armor as one to be calculated
                armorTypes.Add(damage.damageType);

                armorValues.Add(damage.damageType, pawn.GetStatValue(damage.armorType));

                armorOffsets.Add(damage.damageType, 0f);

                armorNewValues.Add(damage.damageType, 0f);

                damageAmounts.Add(damage.damageType, 0f);
            }

            if(!AM_Utilities.Settings.useHeat)
            {
                armorTypes.Remove(AM_DefOf.Heat);

                damageAmounts.Remove(AM_DefOf.Heat);
            }
            else
            {
                usingHeat = true;
            }

            defName = pawn.def.defName;
            maxValue = AM_Utilities.Settings.maxValue;
            adaptationStep = AM_Utilities.Settings.adaptationStep;
        }

        public void ResetArmor()
        {
            bool useHeat = AM_Utilities.Settings.useHeat;
            bool hardMode = AM_Utilities.Settings.hardMode;

            maxValue = AM_Utilities.Settings.maxValue;

            //Adding heat armor if enabled mid-game
            if(useHeat)
            {
                armorTypes.Add(AM_DefOf.Heat);
                armorOffsets.Add(AM_DefOf.Heat, 0f);

                damageAmounts.Add(AM_DefOf.Heat, 0f);
            }
            //Removing heat armor if disabled mid-game
            else if(usingHeat && !useHeat)
            {
                damageAmounts.Remove(AM_DefOf.Heat);

                if(!hardMode)
                {
                    //Refunding points
                    unspentPoints += armorOffsets[AM_DefOf.Heat];
                }
                
                armorOffsets[AM_DefOf.Heat] = 0f;
                armorTypes.Remove(AM_DefOf.Heat);

                usingHeat = false;
            }

            if(!hardMode)
            {
                //Recalculating armor distribution, under assumption that max distribution had been reached before reset
                //No stepping
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = Armor_total*(damageAmounts[armor]/(DamageAmountsTotal));
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        while(armorOffsets[armor] - adaptationStep >= -armorValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }
                while(unspentPoints > 0f)
                {
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        if(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                    }
                }
            }
        }

        public void CalculateArmor(bool hardMode)
        {
            //Calculating armor for regular mode
            if(!hardMode)
            {
                //If heat armor was disabled without reset
                if(usingHeat && !AM_Utilities.Settings.useHeat)
                {
                    damageAmounts.Remove(AM_DefOf.Heat);
                    unspentPoints += armorOffsets[AM_DefOf.Heat];
                    armorOffsets[AM_DefOf.Heat] = 0f;
                    armorTypes.Remove(AM_DefOf.Heat);
                }
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = Armor_total*(damageAmounts[armor]/(DamageAmountsTotal));
                }
                //Subtracting armor points
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        if(armorOffsets[armor] - adaptationStep >= -armorValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }
            }
            //Calculating for hard mode, slight balance adjustment
            else
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = armorValues[armor] + (damageAmounts[armor] * 0.01f);
                }
            }
            //Adding armor points
            if(unspentPoints > 0f || hardMode)
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue)
                    {
                        if((unspentPoints - adaptationStep) >= 0f && !hardMode)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                        else if(hardMode)
                        {
                            //Slower in hard mode
                            armorOffsets[armor] += adaptationStep * 0.1f;
                        }
                    }
                }
            }
        }
    }
}