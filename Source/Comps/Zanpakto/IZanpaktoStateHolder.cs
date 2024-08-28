using System;

namespace JJK
{
    public interface IZanpaktoStateHolder
    {
        ZanpaktoState CurrentState { get; }
        void SetState(ZanpaktoState state);
        event Action<ZanpaktoState> StateChanged;
    }
}