using Verse;

namespace AdaptableMechanoids
{
    public static class AM_Utilities
    {
        public static AM_Settings Settings => LoadedModManager.GetMod<AdaptableMechanoidsMod>().GetSettings<AM_Settings>();
    }
}