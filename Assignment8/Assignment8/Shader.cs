using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Assignment8
{
    public class Shader
    {
        private string VertexShaderText;
        private string FragmentShaderText;
        private int VertexShader;
        private int FragmentShader;
        private int ShaderProgram;
        private Dictionary<string, int> Uniforms;
        public Shader(string VertexShaderPath, string FragmentShaderPath)
        {
            var vertexShaderSource = PreProcessShader(VertexShaderPath);
            VertexShaderText = vertexShaderSource;
            File.WriteAllText("Shaders/PreProcess/vertexShaderProcessed.vert", vertexShaderSource);
            VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, vertexShaderSource);
            CompileShader(VertexShader, vertexShaderSource);
            var fragmentShaderSource = PreProcessShader(FragmentShaderPath);
            FragmentShaderText = fragmentShaderSource;
            File.WriteAllText("Shaders/PreProcess/fragmentShaderProcessed.frag", fragmentShaderSource);
            FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, fragmentShaderSource);
            CompileShader(FragmentShader, fragmentShaderSource);

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
            var totalSize = 0;
            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(ShaderProgram, i, out var size, out _);
                totalSize += size;
                var location = GL.GetUniformLocation(ShaderProgram, key);
                Uniforms.Add(key, location);
            }
            Console.WriteLine($"Shader total uniform size: {totalSize}");
        }

        private static string PreProcessShader(string path)
        {
            var directory = new FileInfo(path).Directory.FullName;
            var lines = File.ReadAllLines(path);
            var includedFiles = new HashSet<string>();
            var regex = new Regex(@"#include +< *([\w]+.glsl) *>");
            var sb = new StringBuilder();
            foreach(var line in ProcessFile(lines))
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
            List<string> ProcessFile(string[] lines)
            {
                var result = new List<string>();
                foreach (var line in lines)
                {
                    var match = regex.Match(line);
                    if (match.Success && !includedFiles.Contains(match.Groups[0].Value))
                    {
                        var fileName = match.Groups[1].Value;
                        includedFiles.Add(fileName);
                        var file = File.ReadAllLines(directory + @"\" + fileName);
                        result.AddRange(ProcessFile(file));
                    }
                    else
                    {
                        result.Add(line);
                    }
                }
                return result;
            }
        }

        private static void CompileShader(int shader, string source)
        {
            // Try to compile the shader
            GL.CompileShader(shader);

            // Check for compilation errors
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}\n\n{source}");
            }
        }

        public void SetStructArray(List<object> structs, string arrayName, string countName)
        {
            for (var i = 0; i < structs.Count; i++)
            {
                SetStruct(structs[i], $"{arrayName}[{i}]");
            }
            SetInt(countName, structs.Count);
        }

        public void SetStruct(object o, string name)
        {
            foreach (var field in o.GetType().GetFields())
            {
                var uName = $"{name}.{field.Name}";
                switch(field.GetValue(o))
                {
                    case int value:
                        SetInt(uName, value);
                        break;
                    case float value:
                        SetFloat(uName, value);
                        break;
                    case bool value:
                        SetBool(uName, value);
                        break;
                    case Vector2 value:
                        SetVector2(uName, value);
                        break;
                    case Vector3 value:
                        SetVector3(uName, value);
                        break;
                    case Matrix3 value:
                        SetUniformMatrix3(uName, value);
                        break;
                    case Vector4 value:
                        SetVector4(uName, value);
                        break;
                    case Color4 value:
                        SetColor4(uName, value);
                        break;
                    case List<object> value:
                        SetStructArray(value, uName, uName + "Count");
                        break;
                    default:
                        SetStruct(field.GetValue(o), uName);
                        break;
                }
            }
        }

        public void Use()
        {
            GL.UseProgram(ShaderProgram);
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

        public void SetInt(string name, int value)
        {
            CheckUniform(name);
            GL.Uniform1(Uniforms[name], value);
        }

        public void SetBool(string name, bool value)
        {
            CheckUniform(name);
            GL.Uniform1(Uniforms[name], Convert.ToInt32(value));
        }

        public void SetVector2(string name, Vector2 value)
        {
            CheckUniform(name);
            GL.Uniform2(Uniforms[name], value);
        }

        public void SetVector3(string name, Vector3 value)
        {
            CheckUniform(name);
            GL.Uniform3(Uniforms[name], value);
        }

        public void SetVector4(string name, Vector4 value)
        {
            CheckUniform(name);
            GL.Uniform4(Uniforms[name], value);
        }

        public void SetColor4(string name, Color4 value)
        {
            CheckUniform(name);
            GL.Uniform4(Uniforms[name], value);
        }
         
        public void SetUniformMatrix4(string name, Matrix4 value)
        {
            CheckUniform(name);
            GL.UniformMatrix4(Uniforms[name], true, ref value);
        }
        public void SetUniformMatrix3(string name, Matrix3 value)
        {
            CheckUniform(name);
            GL.UniformMatrix3(Uniforms[name], true, ref value);
        }

        private void CheckUniform(string name)
        {
            if (!Uniforms.ContainsKey(name))
            {
                var id = GL.GetUniformLocation(ShaderProgram, name);
                if (id == -1)
                    throw new Exception($"Uniform not found: {name}, id returned = {id}");
                Uniforms.Add(name, id);
            }
        }
    }
}
