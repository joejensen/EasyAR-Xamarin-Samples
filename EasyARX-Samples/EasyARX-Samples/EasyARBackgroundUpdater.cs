using EasyAR;
using System;
using Urho;

namespace EasyARX
{
    /// <summary>
    /// Called per-frame to initialize and/or update a material that can be used to render the background
    /// </summary>
    class EasyARBackgroundUpdater
    {
        private PixelFormat initializedFormat = PixelFormat.Unknown;
        private int initializedWidth = 0;
        private int initializedHeight = 0;

        private readonly EasyARBackgroundTexture bgTexture = new EasyARBackgroundTexture();
        private readonly EasyARBackgroundTexture uvTexture = new EasyARBackgroundTexture();
        private readonly EasyARBackgroundTexture vTexture = new EasyARBackgroundTexture();

        public Material Material { get; private set; } = new Material();

        private bool InitTextures(Application application, PixelFormat format, int width, int height)
        {
            bgTexture.Create(application, format, width, height, 0);
            uvTexture.Create(application, format, width, height, 1);
            vTexture.Create(application, format, width, height, 2);

            string shaderName = GetShaderName(format);

            // Update the material
            Technique technique = application.ResourceCache.GetTechnique("Techniques/Diff.xml");
            for (uint i = 0; i < technique.NumPasses; i++)
            {
                Pass pass = technique.GetPass(i);
                if (pass != null)
                {
                    pass.VertexShader = shaderName;
                    pass.PixelShader = shaderName;
                }
            }
            Material.SetTechnique(0, technique);
            bgTexture.SetNameIfValid(Material, TextureUnit.Diffuse);
            uvTexture.SetNameIfValid(Material, TextureUnit.Normal);
            vTexture.SetNameIfValid(Material, TextureUnit.Specular);
            Material.CullMode = CullMode.None;

            this.initializedFormat = format;
            this.initializedWidth = width;
            this.initializedHeight = height;

            return true;
        }

        public bool UpdateTexture(Application application, PixelFormat format, int width, int height, EasyAR.Buffer buffer)
        {
            if (this.initializedFormat != format || this.initializedWidth != width || this.initializedHeight != height)
            {
                if (this.initializedFormat == PixelFormat.Unknown)
                {
                    if (!InitTextures(application, format, width, height))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            bgTexture.Update(buffer);
            uvTexture.Update(buffer);
            vTexture.Update(buffer);
            return true;
        }

        string GetShaderName( PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.Gray:
                case PixelFormat.RGB888:
                case PixelFormat.RGBA8888:
                    return "ARRGB";
                case PixelFormat.BGR888:
                case PixelFormat.BGRA8888:
                    return "ARBGR";
                case PixelFormat.YUV_NV21:
                    return "YUVNV21";
                case PixelFormat.YUV_NV12:
                    return "YUVNV12";
                case PixelFormat.YUV_I420:
                case PixelFormat.YUV_YV12:
                    return "YUVI420YV12";
                default:
                    throw new Exception($"Invalid pixel format: {format}");
            }
        }
    }
}
