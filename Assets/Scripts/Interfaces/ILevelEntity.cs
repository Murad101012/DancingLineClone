using Gameplay;

namespace Interfaces
{
    public interface ILevelEntity
    {
        void OnLevelStart();
        void OnLevelStop();
        void OnLevelRestart();
    }
}