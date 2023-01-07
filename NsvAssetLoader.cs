using System;
using System.Linq;
using System.Reflection;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace SliceVisualizer
{
    internal class NsvAssetLoader : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;

        private Material? _uiNoGlowMaterial;
        private bool _loggedNoGlowMaterial = false;

        public Material? UINoGlowMaterial
        {
            get
            {
                if (_uiNoGlowMaterial != null)
                {
                    return _uiNoGlowMaterial;
                }

                var sprite = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(m => m.name == "GameUISprite");
                if (sprite == null)
                {
                    var shader = Shader.Find("Custom/CustomParticles");
                    if (shader != null)
                    {
                        _siraLog.Info("Found shader, create material from this.");
                        _uiNoGlowMaterial = MakeUiMaterial(shader);
                        return _uiNoGlowMaterial;
                    }
                    if (!_loggedNoGlowMaterial)
                    {
                        _loggedNoGlowMaterial = true;
                        _siraLog.Error("Trying to get GameUISprite before it was loaded. This should not happen.");
                        foreach (var material in Resources.FindObjectsOfTypeAll<Material>())
                        {
                            _siraLog.Debug($"material: {material.name}");
                        }
                    }
                }
                else
                {
                    _uiNoGlowMaterial = new Material(sprite);
                }

                return _uiNoGlowMaterial;
            }
        }

        private Material MakeUiMaterial(Shader shader)
        {
            var material = new Material(shader);
            var keywords = new string[] {
                "DECAL_ON", "ENABLE_VERTEX_COLOR", "ETC1_EXTERNAL_ALPHA", "HEIGHT_FOG", "SQUAREALPHA",
                "SQUARE_ALPHA", "VERTEX_COLOR", "_EMISSION", "_FOGTYPE_ALPHA", "_VERTEXCHANNELS_RGBA",
                "_WHITEBOOSTTYPE_NONE"
            };
            foreach (var keyword in keywords)
            {
                material.EnableKeyword(keyword);
            }
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            return material;
        }

        public Sprite? RRect { get; private set; }
        public Sprite? Circle { get; private set; }
        public Sprite? Arrow { get; private set; }
        public Sprite? White { get; private set; }

        public NsvAssetLoader(SiraLog siraLog)
        {
            _siraLog = siraLog;
        }

        public void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            RRect = LoadSpriteFromResources(assembly, "SliceVisualizer.Assets.RRect.png");
            Circle = LoadSpriteFromResources(assembly, "SliceVisualizer.Assets.Circle.png");
            Arrow = LoadSpriteFromResources(assembly, "SliceVisualizer.Assets.Arrow.png");
            White = LoadSpriteFromResources(assembly, "SliceVisualizer.Assets.White.png", 1f);
        }

        public void Dispose()
        {
            _uiNoGlowMaterial = null;

            RRect = null;
            Circle = null;
            Arrow = null;
            White = null;
        }

        private Sprite? LoadSpriteFromResources(Assembly assembly, string resourcePath, float pixelsPerUnit = 256.0f)
        {
            using var stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                _siraLog.Warn($"Couldn't find embedded resource {resourcePath}");
                return null;
            }

            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int) stream.Length);
            if (imageData.Length == 0)
            {
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            texture.LoadImage(imageData);

            var rect = new Rect(0, 0, texture.width, texture.height);
            var sprite = Sprite.Create(texture, rect, Vector2.zero, pixelsPerUnit);

            _siraLog.Info($"Successfully loaded sprite {resourcePath}, w={texture.width}, h={texture.height}");

            return sprite;
        }
    }
}