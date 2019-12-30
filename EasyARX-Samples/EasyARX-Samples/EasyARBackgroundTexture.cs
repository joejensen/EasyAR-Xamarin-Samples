using EasyAR;
using System;
using System.Collections.Generic;
using Urho;
using Urho.Urho2D;

namespace EasyARX
{
    /// <summary>
    /// Maintains an individual texture used for rendering the camera background
    /// </summary>
    class EasyARBackgroundTexture
    {
        private readonly static Dictionary<Tuple<PixelFormat, int>, EasyArCameraTextureDescriptor> TEXTURE_DESCRIPTORS = new Dictionary<Tuple<PixelFormat, int>, EasyArCameraTextureDescriptor>
        {
            {Tuple.Create(PixelFormat.Gray, 0), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.RGB888, 0), new EasyArCameraTextureDescriptor(Graphics.RGBFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.BGR888, 0), new EasyArCameraTextureDescriptor(Graphics.RGBFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.RGBA8888, 0), new EasyArCameraTextureDescriptor(Graphics.RGBAFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.BGRA8888, 0), new EasyArCameraTextureDescriptor(Graphics.RGBAFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.YUV_NV12, 0), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.YUV_NV21, 0), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.YUV_YV12, 0), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 1, 0, 1)},
            {Tuple.Create(PixelFormat.YUV_I420, 0), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 1, 0, 1)},

            {Tuple.Create(PixelFormat.YUV_NV12, 1), new EasyArCameraTextureDescriptor(Graphics.LuminanceAlphaFormat, 2, 1, 1)},
            {Tuple.Create(PixelFormat.YUV_NV21, 1), new EasyArCameraTextureDescriptor(Graphics.LuminanceAlphaFormat, 2, 1, 1)},
            {Tuple.Create(PixelFormat.YUV_YV12, 1), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 4, 5, 4)},
            {Tuple.Create(PixelFormat.YUV_I420, 1), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 4, 1, 1)},

            {Tuple.Create(PixelFormat.YUV_YV12, 2), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 4, 1, 1)},
            {Tuple.Create(PixelFormat.YUV_I420, 2), new EasyArCameraTextureDescriptor(Graphics.LuminanceFormat, 4, 5, 4)},
        };

        private int textureOffset;
        private int textureWidth;
        private int textureHeight;

        public Texture2D Texture { get; private set; }

        public EasyARBackgroundTexture()
        {
        }

        public bool Create(Application application, PixelFormat format, int width, int height, int layer)
        {
            EasyArCameraTextureDescriptor descriptor;
            if (!TEXTURE_DESCRIPTORS.TryGetValue(Tuple.Create(format, layer), out descriptor))
            {
                return false;
            }

            textureOffset = (int)(width * height * descriptor.LayerOffsetNumerator / descriptor.LayerOffsetDenominator);
            textureWidth = (int)(width / descriptor.ScaleFactor);
            textureHeight = (int)(height / descriptor.ScaleFactor);

            Texture = new Texture2D();
            Texture.SetNumLevels(1);
            Texture.FilterMode = TextureFilterMode.Bilinear;
            Texture.SetAddressMode(TextureCoordinate.U, TextureAddressMode.Clamp);
            Texture.SetAddressMode(TextureCoordinate.V, TextureAddressMode.Clamp);
            Texture.SetSize(textureWidth, textureHeight, descriptor.TextureFormat, TextureUsage.Dynamic);
            Texture.Name = "camBg" + layer;
            application.ResourceCache.AddManualResource(Texture);

            return true;
        }

        public void SetNameIfValid(Material material, TextureUnit unit)
        {
            if (Texture != null)
            {
                material.SetTexture(unit, Texture);
            }
        }

        public unsafe bool Update(EasyAR.Buffer buffer)
        {
            if (Texture != null)
            {
                Texture.SetData(0, 0, 0, textureWidth, textureHeight, (byte*)buffer.data().ToPointer() + textureOffset);
                return true;
            }

            return false;
        }
    }

    class EasyArCameraTextureDescriptor
    {
        public uint TextureFormat { get; private set; }
        public uint ScaleFactor { get; private set; }
        public uint LayerOffsetNumerator { get; private set; }
        public uint LayerOffsetDenominator { get; private set; }

        public EasyArCameraTextureDescriptor(uint textureFormat, uint scaleFactor, uint layerOffsetNumerator, uint layerOffsetDenominator)
        {
            this.TextureFormat = textureFormat;
            this.ScaleFactor = scaleFactor;
            this.LayerOffsetNumerator = layerOffsetNumerator;
            this.LayerOffsetDenominator = layerOffsetDenominator;
        }
    }
}
