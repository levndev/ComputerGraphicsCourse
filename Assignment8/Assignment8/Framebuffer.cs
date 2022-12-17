using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment8
{
    internal class Framebuffer : IDisposable
    {
        public readonly int FBO;
        public readonly Texture TargetTexture;
        public Framebuffer(Texture target)
        {
            TargetTexture = target;
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, target.Handle, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Dispose()
        {
            GL.DeleteFramebuffer(FBO);
        }

        public void Use()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        }
    }
}
