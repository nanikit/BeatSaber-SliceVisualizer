using System;
using System.Reflection;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace SliceVisualizer
{
    internal class NsvAssetLoader : IInitializable, IDisposable
    {
        private Material? _uiNoGlowMaterial;
        private SiraLog _siraLog;

        public NsvAssetLoader(SiraLog siraLog)
        {
            _siraLog = siraLog;

            var bundle = TryLoadAssetBundle();
            if (bundle == null)
            {
                _siraLog.Warn($"Couldn't find shader asset bundle");
                return;
            }

            var shader = bundle.LoadAsset<Shader>("Assets/UnlitGlow.shader");
            _uiNoGlowMaterial = new Material(shader);

            bundle.Unload(false);
        }

        public Material UiNoGlowMaterial => _uiNoGlowMaterial ?? throw new InvalidOperationException("Asset loader not initialized");

        public Sprite? RRect { get; private set; }

        public Sprite? Circle { get; private set; }

        public Sprite? Arrow { get; private set; }

        public Sprite? White { get; private set; }

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

        private AssetBundle? TryLoadAssetBundle()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream("SliceVisualizer.Assets.shaderbundle");
            return stream == null ? null : AssetBundle.LoadFromStream(stream);
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