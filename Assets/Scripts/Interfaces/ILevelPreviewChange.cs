using Gameplay;

namespace Interfaces
{
    /// <summary>
    /// It's a simple contract those change their properties when player change between Level Preview with
    /// <see cref="Ui.Core.LevelPreviewChangeSender.LevelChangePreviousLevelButtonReferenceOnClicked"/> and
    /// <see cref="Ui.Core.LevelPreviewChangeSender.LevelChangeNextLevelButtonReferenceOnClicked"/>
    /// </summary>
    /// <remarks>
    /// Logic that when player change between level previews happen at <see cref="Ui.Core.LevelPreviewChangeSender"/>.
    /// This script simply add contract and can be uses without this, but suggested for Clean Code
    /// </remarks>
    public interface ILevelPreviewChange
    {
        public void OnLevelPreviewChange(LevelPropertiesSo levelPropertiesSo);
    }
}