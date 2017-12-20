using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace FirefoamBelt
{

    public class CompFirefoamBelt : ThingComp
    {

        public CompProperties_FirefoamBelt Props
        {
            get
            {
                return (CompProperties_FirefoamBelt)this.props;
            }
        }

        public override void CompTick()
        {
            Log.Message("Tick - Spawned:" + this.parent.Spawned.ToString() + 
                        " Parent Pos: " + this.parent.Position.ToString() + 
                        " Parent Pos Held: " + this.parent.PositionHeld);
            
            base.CompTick();
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            
            absorbed = true;
            
            Log.Message("PreKill - Spawned: " + this.parent.Spawned.ToString() +
                        " Parent Pos: " + this.parent.Position.ToString() +
                        " Parent Pos Held: " + this.parent.PositionHeld);

            Log.Error("Killing Parent of Comp");

            this.parent.Kill(new DamageInfo?(), (Hediff)null);

            Log.Message("PostKill - Spawned: " + this.parent.Spawned.ToString() +
                        " Parent Pos: " + this.parent.Position.ToString() +
                        " Parent Pos Held: " + this.parent.PositionHeld);
        }
    }

    public class CompProperties_FirefoamBelt : CompProperties
    {

        public CompProperties_FirefoamBelt()
        {
            this.compClass = typeof(CompFirefoamBelt);
        }
    }

}

