using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace FirefoamBelt
{

    public class FirefoamBelt : Apparel
    {
        private float ApparelScorePerBeltRadius = 23.0f / 500.0f;

        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Def == DamageDefOf.Burn || dinfo.Def == DamageDefOf.Flame)
            {
                Log.Message("Fire! Fire!");

                GenExplosion.DoExplosion(this.Wearer.Position, this.Wearer.Map, 5, DamageDefOf.Extinguish, this);
                this.Destroy(DestroyMode.Vanish);
                return true;
            }
            //GenExplosion.DoExplosion(this.Wearer.Position, this.Wearer.Map, StatExtension.GetStatValue((Thing)this, StatDefOf.SmokepopBeltRadius, true), DamageDefOf.Smoke, (Thing)null, -1, (SoundDef)null, (ThingDef)null, (ThingDef)null, ThingDefOf.Gas_Smoke, 1f, 1, false, (ThingDef)null, 0.0f, 1, 0.0f, false);
            return false;
        }


        public override float GetSpecialApparelScoreOffset()
        {
            Log.Message("FirefoamBelt.GetSpecialApparelScoreOffset");
            return StatExtension.GetStatValue((Thing)this, StatDefOf.SmokepopBeltRadius, true) * this.ApparelScorePerBeltRadius;
        }
    }
}

