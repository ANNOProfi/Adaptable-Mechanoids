using Verse;
using RimWorld;

namespace AdaptableMechanoids
{
    [DefOf]
    public static class AM_DefOf
    {
        static AM_DefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(AM_DefOf));
		}

        public static DamageArmorCategoryDef Blunt;

        public static DamageArmorCategoryDef Heat;
    }
}