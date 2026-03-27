using Core;

namespace Interfaces
{
    /// <summary>
    /// If a scene load, a component initialization (Awake(), Start()), depend on other components
    /// and those component aren't ready yet from scene load, then this interface can be used.
    /// For that, those codes are throwing null error or still aren't ready to warm-up, must
    /// move from Awake(), Start() to Initialization() function of IReady.
    /// With this, when scene completely loaded, Initialization() function
    /// will be call by <see cref="SceneLoader.AfterSceneLoad"/> to prevent race-condition
    /// </summary>
    /// <remarks>Proboably will be replaced with DI</remarks>
    public interface IReady
    {
        void Initialization();
    }
}