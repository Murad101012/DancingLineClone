using Gameplay;

namespace Interfaces
{
    /// <summary>
    /// Triggered when the player cycles through level selections to update preview elements.
    /// </summary>
    /// <remarks>
    /// Implementations should extract only the relevant data from <see cref="LevelPropertiesSo"/>.
    /// <example>
    /// <see cref="Ui.Menu.MenuUiController"/> uses <see cref="LevelPropertiesSo.levelImage"/>, 
    /// while <see cref="Audio.GlobalAudioPlayer"/> uses <see cref="LevelPropertiesSo.levelSound"/>.
    /// </example>
    /// </remarks>
    public interface ILevelPreviewChange
    {
        public void OnLevelPreviewChange(LevelPropertiesSo levelPropertiesSo);
    }
}