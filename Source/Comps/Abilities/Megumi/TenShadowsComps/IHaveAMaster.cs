using Verse;

namespace JJK
{
    public interface IHaveAMaster
    {
        Pawn Master { get; }

        void SetMaster(Pawn NewMaster);
    }
}



