using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using LudeonTK;
using UnityEngine;

namespace AdaptableMechanoids
{
    public class AM_MechArmorStats
    {
        private float Armor_total
        {
            get
            {
                float armorTotal = 0f;
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorTotal += armorValues[armor];
                }

                return armorTotal;
            }
        }

        private float NewArmor_total
        {
            get
            {
                float newArmorTotal = 0f;
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    newArmorTotal += armorNewValues[armor];
                }

                return newArmorTotal;
            }
        }

        private float DamageAmountsTotal
        {
            get
            {
                float damageTotal = 0f;
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    damageTotal += damageAmounts[armor];
                }

                return damageTotal;
            }
        }

        private float unspentPoints = 0f;

        private float adaptationStep;

        public List<DamageArmorCategoryDef> armorTypes = new List<DamageArmorCategoryDef>();

        private List<DamageArmorCategoryDef> tickingArmorTypes = new List<DamageArmorCategoryDef>();

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

            if(AM_Utilities.Settings.useMax)
            {
                maxValue = AM_Utilities.Settings.maxValue;
            }
            else
            {
                maxValue = StatDefOf.ArmorRating_Blunt.maxValue;
            }
            
            adaptationStep = AM_Utilities.Settings.adaptationStep;
        }

        public void ResetMax()
        {
            if(AM_Utilities.Settings.useMax)
            {
                maxValue = AM_Utilities.Settings.maxValue;
            }
            else
            {
                maxValue = StatDefOf.ArmorRating_Blunt.maxValue;
            }
        }

        public void ResetArmor(bool debug)
        {
            if(debug)
            {
                Log.Message("AM: Resetting armor");
            }
            bool useHeat = AM_Utilities.Settings.useHeat;
            bool hardMode = AM_Utilities.Settings.hardMode;

            if(AM_Utilities.Settings.useMax)
            {
                maxValue = AM_Utilities.Settings.maxValue;
            }
            else
            {
                maxValue = StatDefOf.ArmorRating_Blunt.maxValue;
            }

            //Adding heat armor if enabled mid-game
            if(debug)
            {
                Log.Message("AM: Checking if heat enabled mid-game");
            }
            
            if(!usingHeat && useHeat)
            {
                Log.Message("AM: Heat enabled mid-game");
                armorTypes.Add(AM_DefOf.Heat);
                armorOffsets.Add(AM_DefOf.Heat, 0f);

                damageAmounts.Add(AM_DefOf.Heat, 0f);

                usingHeat = true;
            }

            if(debug)
            {
                Log.Message("AM: Checking if heat disabled mid-game");
            }
            
            //Removing heat armor if disabled mid-game
            if(usingHeat && !useHeat)
            {
                if(debug)
                {
                    Log.Message("AM: Heat disabled mid-game");
                }
                
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

            if(!hardMode && DamageAmountsTotal > 0f)
            {
                if(debug)
                {
                    Log.Message("AM: Recalculating armor");
                }

                tickingArmorTypes.Clear();
                
                //Recalculating armor distribution, under assumption that max distribution had been reached before reset
                //No stepping
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    tickingArmorTypes.Add(armor);
                    armorNewValues[armor] = Mathf.Clamp(Armor_total*(damageAmounts[armor]/DamageAmountsTotal), 0, maxValue);
                    if(armorNewValues[armor] <= maxValue && armorNewValues[armor] > 0f)
                    {
                        tickingArmorTypes.Remove(armor);
                    }
                    if(debug)
                    {
                        Log.Message("AM: New "+armor+" armor: "+armorNewValues[armor]*100+"%");
                    }
                }
                
                if(NewArmor_total < Armor_total)
                {
                    float points = Armor_total - NewArmor_total;
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        if(armorNewValues[armor] == 0f)
                        {
                            armorNewValues[armor] = points/tickingArmorTypes.Count();
                            if(debug)
                            {
                                Log.Message("AM: New "+armor+" armor: "+armorNewValues[armor]*100+"%");
                            }
                        }
                    }
                }
                else if(NewArmor_total > Armor_total)
                {
                    Log.Error("AM_Error: Armor total of "+defName+" exceeds previous total after recalculation");
                }

                if(debug)
                {
                    Log.Message("AM: Subtracting armor");
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        while(armorOffsets[armor] - adaptationStep >= -armorValues[armor] && armorOffsets[armor] + armorValues[armor] > armorNewValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }

                if(debug)
                {
                    Log.Message("AM: Available points: "+unspentPoints*100+"%");
                    Log.Message("AM: Adding armor");
                }
                if(unspentPoints > 0f)
                {
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        while(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue && (unspentPoints - adaptationStep) >= 0f)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                    }
                }

                if(debug)
                {
                    Log.Message("AM: Points left: "+unspentPoints*100+"%");
                    Log.Message("AM: Reset finished");
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
                //Calculating and subtracting armor points
                /*foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = Armor_total*(damageAmounts[armor]/(DamageAmountsTotal));

                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        if(armorOffsets[armor] - adaptationStep >= -armorValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }*/

                tickingArmorTypes.Clear();

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    tickingArmorTypes.Add(armor);
                    armorNewValues[armor] = Mathf.Clamp(Armor_total*(damageAmounts[armor]/DamageAmountsTotal), 0, maxValue);
                    if(armorNewValues[armor] <= maxValue && armorNewValues[armor] > 0f)
                    {
                        tickingArmorTypes.Remove(armor);
                    }
                }
                
                if(NewArmor_total < Armor_total)
                {
                    float points = Armor_total - NewArmor_total;
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        if(armorNewValues[armor] == 0f)
                        {
                            armorNewValues[armor] = points/tickingArmorTypes.Count();
                        }
                    }
                }
                else if(NewArmor_total > Armor_total)
                {
                    Log.Error("AM_Error: Armor total of "+defName+" exceeds previous total after recalculation");
                }

                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    if(armorNewValues[armor] < armorValues[armor])
                    {
                        while(armorOffsets[armor] - adaptationStep >= -armorValues[armor] && armorOffsets[armor] + armorValues[armor] > armorNewValues[armor])
                        {
                            armorOffsets[armor] -= adaptationStep;
                            unspentPoints += adaptationStep;
                        }
                    }
                }

                //Adding armor points
                while(unspentPoints >= adaptationStep)
                {
                    foreach(DamageArmorCategoryDef armor in armorTypes)
                    {
                        if(armorNewValues[armor] > armorValues[armor] && (armorOffsets[armor] + armorValues[armor]) < maxValue)
                        {
                            armorOffsets[armor] += adaptationStep;
                            unspentPoints -= adaptationStep;
                        }
                    }
                }
            }
            //Calculating for hard mode, slight balance adjustment
            else
            {
                foreach(DamageArmorCategoryDef armor in armorTypes)
                {
                    armorNewValues[armor] = armorValues[armor] + (damageAmounts[armor] * adaptationStep);

                    if(armorNewValues[armor] > armorValues[armor] && armorOffsets[armor] + armorValues[armor] < maxValue)
                    {
                        //Slower in hard mode
                        armorOffsets[armor] += adaptationStep * 0.1f;
                    }
                }
            }
        }
    }
}