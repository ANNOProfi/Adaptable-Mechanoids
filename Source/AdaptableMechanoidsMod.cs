using RimWorld;
using UnityEngine;
using Verse;

namespace AdaptableMechanoids
{
    public class AdaptableMechanoidsMod : Mod
    {
        private AM_Settings settings;

        private string maxValueBuffer = 2f.ToString();

        private string timeBuffer = 5000.ToString();

        private string stepBuffer = 0.001f.ToString();

        public AdaptableMechanoidsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<AM_Settings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listing_Standard = new Listing_Standard();

            listing_Standard.Begin(inRect);

            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("AdaptableMechanoids.Settings.UseHeat".Translate(), ref settings.hardMode, "AdaptableMechanoids.Settings.UseHeatDesc".Translate());

            listing_Standard.Gap(10f);
            listing_Standard.Label("AdaptableMechanoids.Settings.MaxValue".Translate((settings.maxValue*100).ToString()), -1, "AdaptableMechanoids.Settings.MaxValueDesc".Translate());
            //maxValueBuffer = settings.maxValue.ToString();
            listing_Standard.TextFieldNumeric(ref settings.maxValue, ref maxValueBuffer, 0f, StatDefOf.ArmorRating_Blunt.maxValue);

            listing_Standard.Gap(10f);
            listing_Standard.Label("AdaptableMechanoids.Settings.AdaptationTime".Translate(settings.adaptationTime.ToStringTicksToPeriodVerbose()), -1, "AdaptableMechanoids.Settings.AdaptationTimeDesc".Translate());
            //timeBuffer = settings.adaptationTime.ToString();
            listing_Standard.TextFieldNumeric(ref settings.adaptationTime, ref timeBuffer, 1f, 3600000);

            listing_Standard.Gap(10f);
            listing_Standard.Label("AdaptableMechanoids.Settings.AdaptationStep".Translate((settings.adaptationStep*100).ToString()), -1, "AdaptableMechanoids.Settings.AdaptationStepDesc".Translate());
            //stepBuffer = settings.adaptationStep.ToString();
            listing_Standard.TextFieldNumeric(ref settings.adaptationStep, ref stepBuffer, 0.00001f, 0.1f);

            listing_Standard.Gap(10f);
            listing_Standard.CheckboxLabeled("AdaptableMechanoids.Settings.HardMode".Translate(), ref settings.hardMode, "AdaptableMechanoids.Settings.HardModeDesc".Translate());

            listing_Standard.End();
        }

        public override string SettingsCategory()
        {
            return "AdaptableMechanoids";
        }
    }
}