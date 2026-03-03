using Player;

namespace Interfaces
{
    public interface IStateSwitchable
    {
        void ChangeState(MoveState.States[] newStates);
    }
}