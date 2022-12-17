using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment8
{
    public class Texture : IDisposable
    {
        public readonly int Handle;
        public Vector2i Size;

        public static Texture LoadFromFile(string path)
        {
            int handle = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, handle);
            StbImage.stbi_set_flip_vertically_on_load(1);
            Vector2i size;
            using (Stream stream = File.OpenRead(path))
            {
                ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                size = new Vector2i(image.Width, image.Height);
            }
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            //GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return new Texture(handle, size);
        }
        /// <summary>
        /// Side order: Right, Left, Top, Bottom, Front, Back
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Texture LoadCubemap(string[] paths)
        {
            if (paths.Length != 6)
                throw new ArgumentException();
            var sides = new TextureTarget[6]
            {
                TextureTarget.TextureCubeMapPositiveX,
                TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveY,
                TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveZ,
                TextureTarget.TextureCubeMapNegativeZ,
            };
            int texHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, texHandle);
            for (var i = 0; i < 6; i++)
            {
                using (Stream stream = File.OpenRead(paths[i]))
                {
                    var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                    GL.TexImage2D(sides[i], 0, PixelInternalFormat.Rgb, image.Width, image.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, image.Data);
                }
            }
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);
            return new Texture(texHandle, Vector2i.Zero);
        }
        public Texture(int glHandle, Vector2i size)
        {
            Size = size;
            Handle = glHandle;
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }

        public void Use(TextureUnit unit, TextureTarget target = TextureTarget.Texture2D)
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(target, Handle);
        }
    }
}
