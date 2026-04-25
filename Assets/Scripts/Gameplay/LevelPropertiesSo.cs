using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace Gameplay
{
    /// <summary>
    /// This ScriptableObject includes properties for level in both.. in level selection
    /// and while in the game
    /// </summary>
    /// <remarks>It's capabilities increased with LevelPropertiesEditor.cs and recommend to use
    /// "Fetch Data from current active scene" button under LevelProperties's inspector</remarks>
    [CreateAssetMenu(menuName = "ScriptableObjects/LevelProperties")]
    public class LevelPropertiesSo : ScriptableObject
    {
        //TODO: Change to Addressable type loading
        public string levelName;
        [SerializeField] public Sprite levelImage;
        public AudioClip levelSound;
        [HideInInspector] public StyleBackground styleBackgroundLevelImage;
        [HideInInspector] public StyleBackground styleBackgroundBlurredLevelImage;
        public AssetReference sceneLevel;

        //As soon as levelImage add from inspector, thi
        private void OnValidate()
        {
            if (levelImage != null)
            {
                styleBackgroundLevelImage = new StyleBackground(levelImage);

                styleBackgroundBlurredLevelImage = new StyleBackground(BakeBlur(levelImage, 2, 2));
            }
        }
        
        /// <summary>
        /// This script written by GEMINI AI
        /// </summary>
        private Texture2D BakeBlur(Sprite sprite, int blurSize, int scale)
{
    // Ensure scale is at least 1 to avoid division by zero
    scale = Mathf.Max(1, scale);
    
    Texture2D source = sprite.texture;
    Rect r = sprite.rect;

    // 1. Calculate aspect-ratio safe dimensions
    int targetW = Mathf.RoundToInt(r.width / scale);
    int targetH = Mathf.RoundToInt(r.height / scale);

    // 2. Extract the slice from the sheet (RGBA32 to keep original colors for now)
    Texture2D cropped = new Texture2D((int)r.width, (int)r.height, TextureFormat.RGBA32, false);
    Color[] slicePixels = source.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
    cropped.SetPixels(slicePixels);
    cropped.Apply();

    // 3. Downscale using RenderTexture to target dimensions
    // We use RGB565 here for the final output to save 50% RAM
    Texture2D blurred = new Texture2D(targetW, targetH, TextureFormat.RGB565, false);
    RenderTexture rt = RenderTexture.GetTemporary(targetW, targetH);
    
    Graphics.Blit(cropped, rt);
    RenderTexture.active = rt;
    blurred.ReadPixels(new Rect(0, 0, targetW, targetH), 0, 0);
    blurred.Apply();
    
    RenderTexture.active = null;
    RenderTexture.ReleaseTemporary(rt);

    // 4. Perform the Blur Loop
    Color[] pixels = blurred.GetPixels();
    Color[] resultPixels = new Color[pixels.Length];

    for (int y = 0; y < targetH; y++)
    {
        for (int x = 0; x < targetW; x++)
        {
            float red = 0, g = 0, b = 0;
            int count = 0;
            for (int ky = -blurSize; ky <= blurSize; ky++)
            {
                for (int kx = -blurSize; kx <= blurSize; kx++)
                {
                    int nx = Mathf.Clamp(x + kx, 0, targetW - 1);
                    int ny = Mathf.Clamp(y + ky, 0, targetH - 1);
                    Color c = pixels[ny * targetW + nx];
                    red += c.r; g += c.g; b += c.b;
                    count++;
                }
            }
            resultPixels[y * targetW + x] = new Color(red / count, g / count, b / count, 1.0f);
        }
    }

    blurred.SetPixels(resultPixels);
    blurred.Apply();

    // 5. Cleanup
    if (Application.isEditor) DestroyImmediate(cropped);
    else Destroy(cropped);

    return blurred;
}

    }
}