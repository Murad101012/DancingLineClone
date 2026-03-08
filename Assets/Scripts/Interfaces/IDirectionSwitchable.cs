using Player;

namespace Interfaces
{
    public interface IDirectionSwitchable
    {
        void ChangeDirection(DirectionController.Directions[] newStates);
    }
}