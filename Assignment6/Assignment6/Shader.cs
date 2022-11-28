using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment6
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
            CompileShader(VertexShader);

            var fragmentShaderSource = File.ReadAllText(FragmentShaderPath);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);
            CompileShader(FragmentShader);

            ShaderProgram = GL.CreateProgram();
            GL.AttachShader(ShaderProgram, VertexShader);
            GL.AttachShader(ShaderProgram, FragmentShader);
            GL.LinkProgram(ShaderProgram);

            GL.DetachShader(ShaderProgram, VertexShader);
            GL.DetachShader(ShaderProgram, FragmentShader);
            GL.DeleteShader(VertexShader);
            GL.DeleteShader(FragmentShader);

            Uniforms = new Dictionary<string, int>();
            GL.GetProgram(ShaderProgram, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(ShaderProgram, i, out _, out _);
                var location = GL.GetUniformLocation(ShaderProgram, key);
                Uniforms.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        public void Use()
        {
            GL.UseProgram(ShaderProgram);
        }

        public void SetMaterial(Material value)
        {
            CheckUniform("material.ambient");
            GL.Uniform3(Uniforms["material.ambient"], value.Ambient);
            CheckUniform("material.diffuse");
            GL.Uniform3(Uniforms["material.diffuse"], value.Diffuse);
            CheckUniform("material.specular");
            GL.Uniform3(Uniforms["material.specular"], value.Specular);
            CheckUniform("material.shininess");
            GL.Uniform1(Uniforms["material.shininess"], value.Shininess);
            if (value.DiffuseMap != null)
            {
                value.DiffuseMap.Use(TextureUnit.Texture0);
                CheckUniform("material.diffuseMap");
                GL.Uniform1(Uniforms["material.diffuseMap"], 0);
                SetBool("useTexture", true);
            }
            else
            {
                SetBool("useTexture", false);
            }
        }

        public void SetLight(Light value)
        {
            CheckUniform("light.position");
            GL.Uniform3(Uniforms["light.position"], value.Position);
            CheckUniform("light.ambient");
            GL.Uniform3(Uniforms["light.ambient"], value.Ambient);
            CheckUniform("light.diffuse");
            GL.Uniform3(Uniforms["light.diffuse"], value.Diffuse);
            CheckUniform("light.specular");
            GL.Uniform3(Uniforms["light.specular"], value.Specular);
        }

        public void SetUniform3(string name, Vector3 value)
        {
            CheckUniform(name);
            GL.Uniform3(Uniforms[name], value);
        }

        public void SetFloat(string name, float value)
        {
            CheckUniform(name);
            GL.Uniform1(Uniforms[name], value);
        }

        public void SetBool(string name, bool value)
        {
            CheckUniform(name);
            GL.Uniform1(Uniforms[name], Convert.ToInt32(value));
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
