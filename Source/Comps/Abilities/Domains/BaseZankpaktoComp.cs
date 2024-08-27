using Verse;

namespace JJK
{
    public class BaseZankpaktoComp : ThingCompExt
    {
        private ZanpaktoWeapon Parent
        {
            get
            {
                if (parent is ZanpaktoWeapon zanpaktoWeapon)
                {
                    return zanpaktoWeapon;
                }

                Log.Error($"BaseZankpaktoComp attached to {parent.def.defName} which must extend be a 'ZanpaktoWeapon' thingClass.");
                return null;
            }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            Parent.StateChanged += Parent_StateChanged;
        }

        private void Parent_StateChanged(ZanpaktoState obj)
        {
            switch (obj)
            {
                case ZanpaktoState.Sealed:
                    OnSeal();
                    break;
                case ZanpaktoState.Shikai:
                    OnShikai();
                    break;
                case ZanpaktoState.Bankai:
                    OnBankai();
                    break;
            }
        }

        public virtual void OnSeal()
        {

        }

        public virtual void OnShikai()
        {

        }
        public virtual void OnBankai()
        {

        }
    }
}