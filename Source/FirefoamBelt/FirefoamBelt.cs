//Not needed as a Harmony Patch is now Used, but left for future reference


//using RimWorld;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Verse;

//namespace FirefoamBelt
//{

//    public class CompFirefoamBelt : ThingComp
//    {

//        public CompProperties_FirefoamBelt Props
//        {
//            get
//            {
//                return (CompProperties_FirefoamBelt)this.props;
//            }
//        }

//        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
//        {

//            absorbed = false;

//            if (dinfo.Def == DamageDefOf.Burn || dinfo.Def == DamageDefOf.Flame)
//            {

//                //Log.Message("Fire! Fire!");

//                //GenExplosion.DoExplosion(this.Wearer.Position, this.Wearer.Map, 5, DamageDefOf.Extinguish, this);
//                //this.Destroy(DestroyMode.Vanish);
//                //return true;
//                var _Apparal = (Apparel)this.parent;

//                if (_Apparal != null)
//                {
//                    GenExplosion.DoExplosion(_Apparal.Wearer.Position, _Apparal.Wearer.Map, 5, DamageDefOf.Extinguish, _Apparal);
//                    absorbed = true;
//                }
//                else
//                {
//                    Log.Error("CompFirefoamBelt.PostPreApplyDamage failed to get Parent Apparal.");
//                }

//            }
//            //GenExplosion.DoExplosion(this.Wearer.Position, this.Wearer.Map, StatExtension.GetStatValue((Thing)this, StatDefOf.SmokepopBeltRadius, true), DamageDefOf.Smoke, (Thing)null, -1, (SoundDef)null, (ThingDef)null, (ThingDef)null, ThingDefOf.Gas_Smoke, 1f, 1, false, (ThingDef)null, 0.0f, 1, 0.0f, false);
//            //return false;
//        }
//    }

//    public class CompProperties_FirefoamBelt : CompProperties
//    {

//        public CompProperties_FirefoamBelt()
//        {
//            this.compClass = typeof(CompFirefoamBelt);
//        }
//    }

//}

