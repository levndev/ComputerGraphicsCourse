using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment2
{
    public class Shader
    {
        private string VertexShaderPath;
        private string FragmentShaderPath;
        private int VertexShader;
        private int FragmentShader;
        private int ShaderProgram;
        private Dictionary<string, int> Uniforms;
        public Shader(string VertexShaderPath, string FragmentShaderPath)
        {
            this.VertexShaderPath = VertexShaderPath;
            this.FragmentShaderPath = FragmentShaderPath;
            var vertexShaderSource = File.ReadAllText(VertexShaderPath);
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertexShaderSource);
            GL.CompileShader(VertexShader);
            var fragmentShaderSource = File.ReadAllText(FragmentShaderPath);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);
            GL.CompileShader(FragmentShader);
            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);
            Uniforms = new Dictionary<string, int>();
        }

        public void Use()
        {
            GL.UseProgram(ShaderProgram);
        }

        public void SetUniform4(string name, Vector4 value)
        {
            CheckUniform(name);
            GL.Uniform4(Uniforms[name], value);
        }

        public void SetUniform4(string name, Color4 value)
        {
            CheckUniform(name);
            GL.Uniform4(Uniforms[name], value);
        }
         
        public void SetUniformMatrix4(string name, Matrix4 value)
        {
            CheckUniform(name);
            GL.UniformMatrix4(Uniforms[name], true, ref value);
        }

        private void CheckUniform(string name)
        {
            if (!Uniforms.ContainsKey(name))
            {
                Uniforms.Add(name, GL.GetUniformLocation(ShaderProgram, name));
            }
        }
    }
}
