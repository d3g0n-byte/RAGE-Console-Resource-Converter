using Converter.utils;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Converter.openFormats
{
	internal class IV_ShadingGroup
	{
		// тут конвертируются шейдеры из rdr в iv
		// для этого будут новые шейдеры, которые будут функционировать как rdr шейдеры
		static string GetParam(int shaderIndex, string paramName, string[] paramsAsString)
		{
			for (int a = 0; a < RDR_ShaderManager.ShaderParams[shaderIndex].shaderparams.Length; a++)
			{
				if (paramName == RDR_ShaderManager.ShaderParams[shaderIndex].shaderparams[a].name) return paramsAsString[a];
			}
			throw new Exception("param not found");
		}
		public static string ConvertRDRShaderToIV(RageResource.RDRShaderFX fx, EndianBinaryReader br)
		{
			string[] paramAsString;

			string[] samplerBuffer = new string[fx.m_nParamCount];
			Vector4[] vector4Buffer = new Vector4[fx.m_nParamCount];
			int currentPosInSamplerBuffer = 0;
			int currentPosInVector4Buffer = 0;

			for (int c = 0; c < fx.m_nParamCount; c++)
			{
				br.Position = fx.value[c].m_pValue;
				switch (fx.value[c].m_nParamType)
				{
					case 0:
						if (fx.value[c].m_pValue != 0)
							samplerBuffer[currentPosInSamplerBuffer++] = $"{DataUtils.ReadStringAtOffset(ReadRageResource.RDRTextureDefinition(br).m_pName, br)}";
						else samplerBuffer[currentPosInSamplerBuffer++] = $"[null]";
						break;
					case 9:
					case 1:
						if (fx.value[c].m_pValue != 0)
							vector4Buffer[currentPosInVector4Buffer++] = br.ReadVector4();
						break;
				}
			}
			string shaderName;
			if (!RDR_ShaderManager.ShaderNames.shaderNames.TryGetValue(fx.m_dwNameHash, out shaderName))/* shaderName = $"0x{fx.m_dwNameHash.ToString("X8")}";*/
				throw new Exception("unk shader");
			int shaderIndexInShaderManager = RDR_ShaderManager.GetShaderIndex(shaderName);

			paramAsString = new string[RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams.Length];
			currentPosInSamplerBuffer = currentPosInVector4Buffer = 0;
		
			for (int c = 0; c < RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams.Length; c++)
			{
				if (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].skip) continue;
				switch (RDR_ShaderManager.ShaderParams[shaderIndexInShaderManager].shaderparams[c].type)
				{
					case "int":
						paramAsString[c] = $"{vector4Buffer[currentPosInVector4Buffer++].X}";
						break;
					case "float":
						paramAsString[c] = $"{vector4Buffer[currentPosInVector4Buffer++].X}";
						break;
					case "float2":
						paramAsString[c] = ($"{vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
						break;
					case "float3":
						paramAsString[c] = ($"{vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
						break;
					case "float4":
						paramAsString[c] = ($"{vector4Buffer[currentPosInVector4Buffer].X};{vector4Buffer[currentPosInVector4Buffer].Y};{vector4Buffer[currentPosInVector4Buffer].Z};{vector4Buffer[currentPosInVector4Buffer++].W}");
						break;
					case "sampler":
						paramAsString[c] = ($"{samplerBuffer[currentPosInSamplerBuffer++]}");
						break;
					case "bool":
						paramAsString[c] = ($"{vector4Buffer[currentPosInVector4Buffer++].X}");
						break;
					case "float4x3":
						paramAsString[c] = ($"0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0");
						break;
					case "float4x4":
						paramAsString[c] = ($"0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0");
						break;
				}
			}
			StringBuilder paramLine = new StringBuilder();
			switch (shaderName)
			{
				case "rdr2_bump_spec_ambocc_shared":
					paramLine.Append("rdr2_bump_spec_ambocc_shared.sps");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "TextureSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "SpecSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "BumpSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "specularFactor", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "specularColorFactor", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "bumpiness", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "Colors", paramAsString)}");
					break;
				case "rdr2_low_lod_nodirt":
					paramLine.Append("rdr2_low_lod_nodirt.sps");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "TextureSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "Colors", paramAsString)}");
					break;
				case "rdr2_window_glow":
					paramLine.Append("gta_emissive.sps");
					paramLine.Append($" rdr2_door_glow_NO_TXD 0.5 1.0");
					break;
				case "rdr2_door_glow":
					paramLine.Append("gta_emissive.sps");
					paramLine.Append($" rdr2_door_glow_NO_TXD 0.5 1.0");
					break;
				case "rdr2_low_lod_nodirt_singlesided":
					paramLine.Append("rdr2_low_lod_nodirt_singlesided.sps");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "TextureSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "Colors", paramAsString)}");
					break;
				case "rdr2_diffuse":
					paramLine.Append("rdr2_diffuse.sps");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "TextureSampler", paramAsString)}");
					paramLine.Append($" {GetParam(shaderIndexInShaderManager, "Colors", paramAsString)}");
					break;
				default:
					throw new Exception($"unk shader {shaderName}");
			}


			return paramLine.ToString();
		}
	}
}
