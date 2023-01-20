using ConsoleApp1;
using Converter.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Converter.RageResource;
using static Converter.ResourceUtils;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Converter
{
	internal class RageResource
	{
		public struct FragmentDictionary
		{
			public uint _vmt;
			public uint m_pBlockMapAdress;
			public uint m_pDrawable;
			public uint m_pTextureDictionary;
		}


		// iv
		public struct Fragment
		{
			public uint _vmt;
			public uint m_dwBlockMapAdress;
			public float _f8;
			public float _fC;
			public float _f10;
			public float _f14;
			public float _f18;
			public float _f1C;
			public float _f20;
			public float _f24;
			public float _f28;
			public float _f2C;
			public float _f30;
			public float _f34;
			public float _f38;
			public float _f3C;
			public Vector4 m_UnbrokenCGOffset;
			public Vector4 m_DampingLinearC;
			public Vector4 m_DampingLinearV;
			public Vector4 m_DampingLinearV2;
			public Vector4 m_DampingAngularC;
			public Vector4 m_DampingAngularV;
			public Vector4 m_DampingAngularV2;
			public uint m_pName;
			public uint m_pDrawable;
			public uint _fB8;
			public uint _fBC;
			public uint _fC0;
			public uint _fC4;// -1
			public uint _fC8;
			public uint m_pChildGroupName;
			public uint _fD0; // к оффетам к нулям
			public uint _fD4; // к оффетам к неизвестной секции
			public uint _fD8;
			public uint _fDC;
			public uint _fE0;
			//public uint _fE0;


		}
		public struct IV_TextureDefinition
		{
			public uint vtable;
			public uint _f4;
			public ushort _f8;
			public ushort _fA;
			public uint _fC;
			public uint _f10;
			public uint m_pName;
			public uint _f18;
		}
		public struct Matrix34
		{
			public Vector4 m0, m1, m2;

		}
		public struct IV_ShaderGroup
		{
			public uint vtable;
			public uint m_pTexture;
			public Collection m_pShaders;
			public Matrix34 _f10;
			public Collection m_pVertexFormat;
			public Collection m_pIndexMapping;
		}
		public struct IV_ShaderFX
		{
			public uint vtable;
			public uint m_dwBlockMapAdress;// поинтер
			public sbyte _f8;
			public byte m_nDrawBucket;
			public sbyte _fA;
			public sbyte _fB;
			public short _fC;
			public ushort m_wIndex;
			public uint _f10;
			public uint m_pShaderParams;
			public uint _f18;
			public uint m_dwParamsCount;
			public uint m_dwEffectSize;
			public uint m_pParameterTypes;
			public uint m_dwHash;//какой-то хеш. Он разный в разных играх и в разных пратформах.
			public uint _f2C;
			public uint _f30;
			public uint m_pParamsHash;
			public uint _f38;
			public uint _f3C;
			public uint _f40;
			public uint m_pName;
			public uint m_pSPS;
			public uint _f4C;// поинтер
			public uint _f50;// поинтер
			public uint _f54;
			public uint _f58;
		}


		public struct XTDHeader
		{
			public uint m_dwVTable;
			public uint m_pBlockMap;
			public uint m_pParentDictionary;
			public uint m_dwUsageCount;
			public RageResource.Collection m_cHash;
			public RageResource.Collection m_cTexture;
		}
		public struct Texture					// only for xbox
		{
			public uint _vmt;
			public int _f4;
			public sbyte _f8;
			public sbyte _f9;
			public short _fA;
			public int _fC;
			public uint _f10;					// pointer
			public int _f14;					// 2 short значения
			public uint m_pName;
			public uint m_pBitmap;
			public ushort m_dwWidth;
			public ushort m_dwHeight;
			public uint m_Lod;
			public float _f28;
			public float _f2C;
			public float _f30;
			public float _f34;
			public float _f38;
			public float _f3C;
		}
		public struct BitMap
		{
			public int _f0;
			public int _f4;
			public int _f8;
			public int _fC;
			public int _f10;
			public int _f14;
			public int _f18;
			public int _f1C;
			public uint m_pPixelData;
			public int _f24;
			public int _f28;
			public int _f2C;
			public int _f30;
			public uint m_dwTextureType;		// has nothing to do with structure
		}
		public struct RDRBone
		{
			public uint m_pName;
			public uint m_dwFlags;
			public uint m_pNextSibling;
			public uint m_pFirstChild;
			public uint m_pParent;
			public ushort m_wBoneIndex;
			public ushort m_wBoneId;
			public ushort m_wMirror;
			public sbyte _f1A;
			public sbyte _f1B;
			public sbyte _f1C;
			public sbyte[] __pad_1D;
			public Vector4 m_vOffset;
			public Vector4 m_vRotationEuler;
			public Vector4 m_vRotationQuaternion;
			public Vector4 m_vScale;
			public Vector4 m_vWorldOffset;
			public Vector4 m_vOrient;
			public Vector4 m_vSorient;
			public Vector4 m_vTransMin;
			public Vector4 m_vTransMax;
			public Vector4 m_vRotMin;
			public Vector4 m_vRotMax;
			public int _fD0;
			public  int _fD4;
			public int _fD8;
			public int _fDC;
			public string[] flagsAsString;
		}
		public struct RDRSkeletonData
		{
			public uint m_pBone;
			public uint m_pChildrenMapping;
			public int _f8;
			public int _fC;
			public int _f10;
			public ushort m_wBoneCount;
			public short _f16;
			public short _f18;
			public short _f1A;
			public uint _f1C;					// flags
			public Collection m_pBoneIdMapping;
			public short m_wUsageCount;
			public short _f2A;
			public uint _f2C;
			public uint _f30;
			public string[] flagsAsString;
			//joint
		}
		public struct RDRTextureDefinition
		{
			public uint _vmt;
			public int _f4;
			public int _f8;
			public short _fC;
			public short _fD;
			public int _f20;
			public int _f24;
			public uint m_pName;
			public int _f2C;
		}

		public struct RDRShaderGroup
		{
			public uint _vmt;					// 0x409a6d00
			public uint m_pTextureDictionary;	// xtd file
			public Collection m_cShaderFX;
		}
		public struct RDRShaderFX
		{
			public uint m_pValue;
			public uint m_dwNameHash;
			public byte m_nParamCount;
			public sbyte _f9;
			public short _fA;
			public int _fC;
			public int _f20;
			public int _f24;
			public int _f28;
			public int _f2C;
			public RDRShaderValue[] value;      // has nothing to do with structure
		}
		public struct RDRShaderValue
		{
			public byte m_nParamType;
			public byte m_nType;				// from v2saconv
			public short _f2;
			public uint m_pValue;
		}
		public struct RDRVolumeData
		{
			public uint _vmt;
			public uint pBlockMapAdress;
			public uint _vmt2;
			public uint pBlockMapAdress2;
			public uint pParentDictionary;
			public int dwUsageCount;			//1
			public Collection cNameHash;
			public Collection cDrawable;
			public uint pTexture;				// xtd файл
			public int _f2C;
			public int _f30;
			public int _f34;
			public int _f38;
		}
		public struct Collection
		{
			public uint m_pList;
			public ushort m_wCount;
			public ushort m_wSize;
		}
		public struct Drawable
		{
			public uint _vmt;
			public uint m_pEndOfTheHeader;
			public uint m_pShaderGroup;
			public uint m_pSkeleton;
			public Vector4 m_vCenter;
			public Vector4 m_vAabbMin;
			public Vector4 m_vAabbMax;
			public uint[] m_pModelCollection;
			public Vector4 m_vDrawDistance;
			public int[] m_dwObjectCount;
			public Vector4 m_vRadius;
			public Collection m_p2DFX;
		}
		public struct Model
		{
			public uint _vmt;
			public Collection m_pGeometry;
			public uint m_pBounds;
			public uint m_pShaderMapping;
			public byte m_nBoneCount;
			public bool m_bSkinned;
			public sbyte _f16;
			public byte m_nBoneIndex;
			public sbyte _f18;
			public byte m_bHasOffset;
			public byte m_nShaderMappingsCount;
		}
		public struct Geometry
		{
			public uint vtable;
			public uint m_piVertexDeclaration;
			public int _f8;
			public uint m_pVertexBuffer;
			public int _f10;
			public int _f14;
			public int _f18;
			public uint m_pIndexBuffer;
			public int _f20;
			public int _f24;
			public int _f28;
			public uint m_dwIndexCount;
			public uint m_dwFaceCount;
			public ushort m_wVertexCount;
			public ushort m_wIndicesPerFace;
			public uint m_pBoneMapping;
			public ushort m_wVertexStride;
			public ushort m_wBoneCount;
			public uint m_pVertexDeclaration;
			public uint _f44;
			public int _f48;
		}
		public struct IndexBuffer
		{
			public uint vtable;
			public uint m_dwIndexCount;
			public uint m_pIndexData;
		}
		public struct rageD3D9VertexFlags
		{
			public bool m_bPosition;
			public bool m_bBlendWeight;
			public bool m_bBlendIndices;
			public bool m_bNormal;
			public bool m_bColor;
			public bool m_bSpecular;
			public bool m_bTexCoord0;
			public bool m_bTexCoord1;
			public bool m_bTexCoord2;
			public bool m_bTexCoord3;
			public bool m_bTexCoord4;
			public bool m_bTexCoord5;
			public bool m_bTexCoord6;
			public bool m_bTexCoord7;
			public bool m_bTangent;
			public bool m_bBinormal;
		}
		public struct rageD3D9VertexElementTypes
		{
			public byte m_nPositionType;
			public byte m_nBlendWeightType;
			public byte m_nBlendIndicesType;
			public byte m_nNormalType;
			public byte m_nColorType;
			public byte m_nSpecularType;
			public byte m_nTexCoord0Type;
			public byte m_nTexCoord1Type;
			public byte m_nTexCoord2Type;
			public byte m_nTexCoord3Type;
			public byte m_nTexCoord4Type;
			public byte m_nTexCoord5Type;
			public byte m_nTexCoord6Type;
			public byte m_nTexCoord7Type;
			public byte m_nTangentType;
			public byte m_nBinormalType;
		}
		public struct VertexDeclaration
		{
			public rageD3D9VertexFlags m_UsedElements;
			public byte m_nTotaSize;
			public sbyte _f6;
			public byte m_bStoreNormalsDataFirst;
			public byte m_nElementsCount;
			public rageD3D9VertexElementTypes m_ElementTypes;
		}
		public struct VertexBuffer
		{
			public uint vtable;
			public ushort m_wVertexCount;
			public byte m_bLocked;
			public sbyte _f7;
			public uint m_pLockedData;
			public uint m_dwVertexSize;
			public uint m_pVertexData;
			public int _f14;
			public uint m_pDeclaration;
		}

	}
	internal class ReadRageResource
	{
		public static RDRVolumeData RDRVolumeData(EndianBinaryReader br)
		{
			RDRVolumeData volumeData;
			volumeData._vmt = br.ReadUInt32();
			volumeData.pBlockMapAdress = br.ReadOffset();
			volumeData._vmt2 = br.ReadUInt32();
			volumeData.pBlockMapAdress2 = br.ReadOffset();
			volumeData.pParentDictionary = br.ReadOffset();
			volumeData.dwUsageCount = br.ReadInt32();
			volumeData.cNameHash.m_pList = br.ReadOffset();
			volumeData.cNameHash.m_wCount = br.ReadUInt16();
			volumeData.cNameHash.m_wSize = br.ReadUInt16();
			volumeData.cDrawable.m_pList = br.ReadOffset();
			volumeData.cDrawable.m_wCount = br.ReadUInt16();
			volumeData.cDrawable.m_wSize = br.ReadUInt16();
			volumeData.pTexture = br.ReadOffset();
			volumeData._f2C = br.ReadInt32();
			volumeData._f30 = br.ReadInt32();
			volumeData._f34 = br.ReadInt32();
			volumeData._f38 = br.ReadInt32();
			return volumeData;
		}
		public static Drawable Drawable(EndianBinaryReader br)
		{
			Drawable drawable;
			drawable._vmt = br.ReadUInt32();
			drawable.m_pEndOfTheHeader = br.ReadOffset();
			drawable.m_pShaderGroup = br.ReadOffset();
			drawable.m_pSkeleton = br.ReadOffset();
			drawable.m_vCenter = br.ReadVector4();
			drawable.m_vAabbMin = br.ReadVector4();
			drawable.m_vAabbMax = br.ReadVector4();

			drawable.m_pModelCollection = new uint[4];
			for (int a = 0; a < 4; a++) drawable.m_pModelCollection[a] = br.ReadOffset();
			drawable.m_vDrawDistance = br.ReadVector4();

			drawable.m_dwObjectCount = new int[4];
			for (int a = 0; a < 4; a++) drawable.m_dwObjectCount[a] = br.ReadInt32();
			drawable.m_vRadius = br.ReadVector4();
			drawable.m_p2DFX.m_pList = br.ReadOffset();
			drawable.m_p2DFX.m_wCount = br.ReadUInt16();
			drawable.m_p2DFX.m_wSize = br.ReadUInt16();
			return drawable;
		}
		public static Model Model(EndianBinaryReader br)
		{
			Model model;
			model._vmt = br.ReadUInt32();
			model.m_pGeometry.m_pList = br.ReadOffset();
			model.m_pGeometry.m_wCount = br.ReadUInt16();
			model.m_pGeometry.m_wSize = br.ReadUInt16();
			model.m_pBounds = br.ReadOffset();
			model.m_pShaderMapping = br.ReadOffset();
			model.m_nBoneCount = br.ReadByte();
			model.m_bSkinned = br.ReadBoolean(); //skinned
			model._f16 = br.ReadSByte();
			model.m_nBoneIndex = br.ReadByte();
			model._f18 = br.ReadSByte();
			model.m_bHasOffset = br.ReadByte();
			model.m_nShaderMappingsCount = br.ReadByte();
			return model;
		}
		public static Geometry Geometry(EndianBinaryReader br)
		{
			Geometry geometry;
			geometry.vtable = br.ReadUInt32();
			geometry.m_piVertexDeclaration = br.ReadUInt32();
			geometry._f8 = br.ReadInt32();
			geometry.m_pVertexBuffer = br.ReadOffset();
			geometry._f10 = br.ReadInt32();
			geometry._f14 = br.ReadInt32();
			geometry._f18 = br.ReadInt32();
			geometry.m_pIndexBuffer = br.ReadOffset();
			geometry._f20 = br.ReadInt32();
			geometry._f24 = br.ReadInt32();
			geometry._f28 = br.ReadInt32();
			geometry.m_dwIndexCount = br.ReadUInt32();
			geometry.m_dwFaceCount = br.ReadUInt32(); // в openiv это используется для блокировки моделей
			geometry.m_wVertexCount = br.ReadUInt16();
			geometry.m_wIndicesPerFace = br.ReadUInt16();
			geometry.m_pBoneMapping = br.ReadOffset();
			geometry.m_wVertexStride = br.ReadUInt16();
			geometry.m_wBoneCount = br.ReadUInt16();
			geometry.m_pVertexDeclaration = br.ReadOffset();
			geometry._f44 = br.ReadOffset();
			geometry._f48 = br.ReadInt32();
			return geometry;
		}
		public static VertexBuffer VertexBuffer(EndianBinaryReader br)
		{
			VertexBuffer vertexBuffer;
			vertexBuffer.vtable = br.ReadUInt32();
			vertexBuffer.m_wVertexCount = br.ReadUInt16();
			vertexBuffer.m_bLocked = br.ReadByte();
			vertexBuffer._f7 = br.ReadSByte();
			vertexBuffer.m_pLockedData = br.ReadOffset(); // к вертексам. Игрой не используется
			vertexBuffer.m_dwVertexSize = br.ReadUInt32();
			vertexBuffer.m_pVertexData = br.ReadOffset();
			vertexBuffer._f14 = br.ReadInt32();
			vertexBuffer.m_pDeclaration = br.ReadOffset();
			return vertexBuffer;
		}
		public static IndexBuffer IndexBuffer(EndianBinaryReader br)
		{
			IndexBuffer indexBuffer;
			indexBuffer.vtable = br.ReadUInt32();
			indexBuffer.m_dwIndexCount = br.ReadUInt32();
			indexBuffer.m_pIndexData = br.ReadOffset();
			return indexBuffer;
		}
		static RageResource.rageD3D9VertexFlags ReadVertexFlags(uint dwFlags)
		{
			RageResource.rageD3D9VertexFlags vertexFlags;
			ushort flags = (ushort)dwFlags;
			vertexFlags.m_bPosition = DataUtils.GetBit(flags, 0);
			vertexFlags.m_bBlendWeight = DataUtils.GetBit(flags, 1);
			vertexFlags.m_bBlendIndices = DataUtils.GetBit(flags, 2);
			vertexFlags.m_bNormal = DataUtils.GetBit(flags, 3);
			vertexFlags.m_bColor = DataUtils.GetBit(flags, 4);
			vertexFlags.m_bSpecular = DataUtils.GetBit(flags, 5);
			vertexFlags.m_bTexCoord0 = DataUtils.GetBit(flags, 6);
			vertexFlags.m_bTexCoord1 = DataUtils.GetBit(flags, 7);
			vertexFlags.m_bTexCoord2 = DataUtils.GetBit(flags, 8);
			vertexFlags.m_bTexCoord3 = DataUtils.GetBit(flags, 9);
			vertexFlags.m_bTexCoord4 = DataUtils.GetBit(flags, 10);
			vertexFlags.m_bTexCoord5 = DataUtils.GetBit(flags, 11);
			vertexFlags.m_bTexCoord6 = DataUtils.GetBit(flags, 12);
			vertexFlags.m_bTexCoord7 = DataUtils.GetBit(flags, 13);
			vertexFlags.m_bTangent = DataUtils.GetBit(flags, 14);
			vertexFlags.m_bBinormal = DataUtils.GetBit(flags, 15);
			return vertexFlags;
		}
		static RageResource.rageD3D9VertexElementTypes ReadVertexElementTypes(ulong Types)
		{
			RageResource.rageD3D9VertexElementTypes vertexElementTypes;
			//			vertexElementTypes.m_nPositionType = (byte)(Types & 0xF000000000000000)>>60;

			byte tmpValue = (byte)((Types & 0xFF00000000000000) >> 56);
			vertexElementTypes.m_nBinormalType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTangentType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x00FF000000000000) >> 48);
			vertexElementTypes.m_nTexCoord7Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord6Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x0000FF0000000000) >> 40);
			vertexElementTypes.m_nTexCoord5Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord4Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x000000FF00000000) >> 32);
			vertexElementTypes.m_nTexCoord3Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord2Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x00000000FF000000) >> 24);
			vertexElementTypes.m_nTexCoord1Type = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nTexCoord0Type = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x0000000000FF0000) >> 16);
			vertexElementTypes.m_nSpecularType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nColorType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)((Types & 0x000000000000FF00) >> 8);
			vertexElementTypes.m_nNormalType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nBlendIndicesType = (byte)((tmpValue & 0xF0) >> 4);

			tmpValue = (byte)(Types & 0x00000000000000FF);
			vertexElementTypes.m_nBlendWeightType = (byte)(tmpValue & 0x0F);
			vertexElementTypes.m_nPositionType = (byte)((tmpValue & 0xF0) >> 4);
			return vertexElementTypes;
		}
		public static VertexDeclaration VertexDeclaration (EndianBinaryReader br)
		{
			VertexDeclaration vertexDeclaration;
			vertexDeclaration.m_UsedElements = ReadVertexFlags(br.ReadUInt32());
			vertexDeclaration.m_nTotaSize = br.ReadByte();
			vertexDeclaration._f6 = br.ReadSByte();
			vertexDeclaration.m_bStoreNormalsDataFirst = br.ReadByte();
			vertexDeclaration.m_nElementsCount = br.ReadByte();
			vertexDeclaration.m_ElementTypes = ReadVertexElementTypes(br.ReadUInt64());
			return vertexDeclaration;
		}
		public static RDRShaderGroup RDRShaderGroup(EndianBinaryReader br)
		{
			RDRShaderGroup shaderGroup;
			shaderGroup._vmt = br.ReadUInt32();
			shaderGroup.m_pTextureDictionary = br.ReadOffset();
			shaderGroup.m_cShaderFX.m_pList = br.ReadOffset();
			shaderGroup.m_cShaderFX.m_wCount = br.ReadUInt16();
			shaderGroup.m_cShaderFX.m_wSize = br.ReadUInt16();
			return shaderGroup;
		}
		public static RDRShaderFX RDRShaderFX(EndianBinaryReader br)
		{
			RDRShaderFX shaderFX;
			shaderFX.m_pValue = br.ReadOffset();
			shaderFX.m_dwNameHash= br.ReadUInt32();
			shaderFX.m_nParamCount = br.ReadByte();
			shaderFX._f9 = br.ReadSByte();
			shaderFX._fA = br.ReadInt16();
			shaderFX._fC = br.ReadInt32();
			shaderFX._f20 = br.ReadInt32();
			shaderFX._f24 = br.ReadInt32();
			shaderFX._f28 = br.ReadInt32();
			shaderFX._f2C = br.ReadInt32();

			shaderFX.value = new RDRShaderValue[shaderFX.m_nParamCount];
			for (int a = 0; a < shaderFX.m_nParamCount; a++)
			{
				br.Position = shaderFX.m_pValue+(a*8);
				shaderFX.value[a] = ReadRageResource.RDRShaderValue(br);
			}
			return shaderFX;
		}
		public static RDRShaderValue RDRShaderValue(EndianBinaryReader br)
		{
			RDRShaderValue value;
			value.m_nParamType = br.ReadByte();
			value.m_nType = br.ReadByte();
			value._f2 = br.ReadInt16();
			value.m_pValue = br.ReadOffset();
			return value;
		}
		public static RDRTextureDefinition RDRTextureDefinition(EndianBinaryReader br)
		{
			RDRTextureDefinition textureDefinition;
			textureDefinition._vmt = br.ReadUInt32();
			textureDefinition._f4 = br.ReadInt32();
			textureDefinition._f8 = br.ReadInt32();
			textureDefinition._fC = br.ReadInt16();
			textureDefinition._fD = br.ReadInt16();
			textureDefinition._f20 = br.ReadInt32();
			textureDefinition._f24 = br.ReadInt32();
			textureDefinition.m_pName = br.ReadOffset();
			textureDefinition._f2C = br.ReadInt32();
			return textureDefinition;
		}
//		public static string GetSkelFlag(byte)
		public static RDRSkeletonData RDRSkeletonData(EndianBinaryReader br)
		{
			RDRSkeletonData skeletonData;
			skeletonData.m_pBone = br.ReadOffset();
			skeletonData.m_pChildrenMapping = br.ReadOffset();
			skeletonData._f8 = br.ReadInt32();
			skeletonData._fC = br.ReadInt32();
			skeletonData._f10 = br.ReadInt32();
			skeletonData._f16 = br.ReadInt16();//
			skeletonData._f16 = br.ReadInt16();//
			skeletonData.m_wBoneCount = br.ReadUInt16();// 
			skeletonData._f18 = br.ReadInt16();
			skeletonData._f1A = br.ReadInt16();
			br.ReadInt16();// skip
			skeletonData._f1C = br.ReadUInt32();// флaги.
			skeletonData.m_pBoneIdMapping.m_pList = br.ReadOffset();
			skeletonData.m_pBoneIdMapping.m_wCount = br.ReadUInt16();
			skeletonData.m_pBoneIdMapping.m_wSize = br.ReadUInt16();
			skeletonData.m_wUsageCount = br.ReadInt16();
			skeletonData._f2A = br.ReadInt16();
			skeletonData._f2C = br.ReadUInt32();
			skeletonData._f30 = br.ReadUInt32();

			uint flagsCount = 0;
			for (int a = 0; a < 32; a++) if (DataUtils.GetBit(skeletonData._f1C, a)) flagsCount++;
			skeletonData.flagsAsString = new string[flagsCount];
			uint currentFlags = 0;
			for (int a = 0; a < 32; a++)
			{
				string flag = "";
				switch (a)
				{
					case 1:
						flag = "HaveBoneMappings";
						break;
					case 2:
						flag = "HaveBoneWorldOrient";
						break;
					case 3:
						flag = "AuthoredOrientation";
						break;
					default:
						flag = $"UnknownFlag{a}";
						break;
				}
				if (DataUtils.GetBit(skeletonData._f1C, a)) skeletonData.flagsAsString[currentFlags++] = flag;
			}
			return skeletonData;
		}
		public static RDRBone RDRBone(EndianBinaryReader br)
		{
			RDRBone bone;
			bone.m_pName = br.ReadOffset();
			bone.m_dwFlags = br.ReadUInt32();
			bone.m_pNextSibling = br.ReadOffset();
			bone.m_pFirstChild = br.ReadOffset();
			bone.m_pParent = br.ReadOffset();
			bone.m_wBoneIndex = br.ReadUInt16();
			bone.m_wBoneId = br.ReadUInt16();
			bone.m_wMirror = br.ReadUInt16();
			bone._f1A = br.ReadSByte();
			bone._f1B = br.ReadSByte();
			bone._f1C = br.ReadSByte();
			bone.__pad_1D = new sbyte[3];
			for (int a = 0; a < 3; a++) bone.__pad_1D[a] = br.ReadSByte();
			bone.m_vOffset = br.ReadVector4();
			bone.m_vRotationEuler = br.ReadVector4();
			bone.m_vRotationQuaternion = br.ReadVector4();
			bone.m_vScale = br.ReadVector4();
			bone.m_vWorldOffset = br.ReadVector4();
			bone.m_vOrient = br.ReadVector4();
			bone.m_vSorient = br.ReadVector4();
			bone.m_vTransMin = br.ReadVector4();
			bone.m_vTransMax = br.ReadVector4();
			bone.m_vRotMin = br.ReadVector4();
			bone.m_vRotMax = br.ReadVector4();
			bone._fD0 = br.ReadInt32();
			bone._fD4 = br.ReadInt32();
			bone._fD8 = br.ReadInt32();
			bone._fDC = br.ReadInt32();
			uint flagsCount=0;
			for (int a = 0; a < 32; a++)if (DataUtils.GetBit(bone.m_dwFlags,a))flagsCount++;
			bone.flagsAsString = new string[flagsCount];
			uint currentFlags = 0;
			for (int a = 0; a < 32; a++)
			{
				string flag="";
				switch (a)
				{
					case 0:
						flag = "LockRotXYZ";
						break;
					case 1:
						flag = "LockRotX";
						break;
					case 2:
						flag = "LockRotY";
						break;
					case 3:
						flag = "LockRotZ";
						break;
					case 4:
						flag = "LimitRotX";
						break;
					case 5:
						flag = "LimitRotY";
						break;
					case 6:
						flag = "LimitRotZ";
						break;
					case 7:
						flag = "LockTransX";
						break;
					case 8:
						flag = "LockTransY";
						break;
					case 9:
						flag = "LockTransZ";
						break;
					case 10:
						flag = "LimitTransX";
						break;
					case 11:
						flag = "LimitTransY";
						break;
					case 12:
						flag = "LimitTransZ";
						break;
					case 13:
						flag = "LockScaleX";
						break;
					case 14:
						flag = "LockScaleY";
						break;
					case 15:
						flag = "LockScaleZ";
						break;
					case 16:
						flag = "LimitScaleX";
						break;
					case 17:
						flag = "LimitScaleY";
						break;
					case 18:
						flag = "LimitScaleZ";
						break;
					case 19:
						flag = "Invisible";
						break;
					default:
						flag = $"UnknownFlag{a}";
						break;
				}
				if (DataUtils.GetBit(bone.m_dwFlags, a)) bone.flagsAsString[currentFlags++] = flag;
			}
			return bone;
		}
		public static XTDHeader XTDHeader(EndianBinaryReader br)
		{
			XTDHeader header;
			header.m_dwVTable = br.ReadUInt32();
			header.m_pBlockMap= br.ReadOffset();
			header.m_pParentDictionary = br.ReadOffset();
			header.m_dwUsageCount = br.ReadUInt32();
			header.m_cHash = br.ReadCollections();
			header.m_cTexture = br.ReadCollections();
			return header;
		}
		public static Texture Texture(EndianBinaryReader br)
		{
			Texture texture;
			texture._vmt= br.ReadUInt32();
			texture._f4 = br.ReadInt32();
			texture._f8 = br.ReadSByte();
			texture._f9 = br.ReadSByte();
			texture._fA = br.ReadInt16();
			texture._fC = br.ReadInt32();
			texture._f10 = br.ReadOffset();
			texture._f14 = br.ReadInt32();
			texture.m_pName = br.ReadOffset();
			texture.m_pBitmap = br.ReadOffset();
			texture.m_dwWidth = br.ReadUInt16();
			texture.m_dwHeight = br.ReadUInt16();
			texture.m_Lod = br.ReadUInt32();
			texture._f28 = br.ReadSingle();
			texture._f2C = br.ReadSingle();
			texture._f30 = br.ReadSingle();
			texture._f34 = br.ReadSingle();
			texture._f38 = br.ReadSingle();
			texture._f3C = br.ReadSingle();
			return texture;
		}
		public static BitMap BitMap(EndianBinaryReader br)
		{
			BitMap bitMap;
			bitMap._f0 = br.ReadInt32();
			bitMap._f4 = br.ReadInt32();
			bitMap._f8 = br.ReadInt32();
			bitMap._fC = br.ReadInt32();
			bitMap._f10 = br.ReadInt32();
			bitMap._f14 = br.ReadInt32();
			bitMap._f18 = br.ReadInt32();
			bitMap._f1C = br.ReadInt32();
			bitMap.m_pPixelData = br.ReadUInt32();
			bitMap._f24 = br.ReadInt32();
			bitMap._f28 = br.ReadInt32();
			bitMap._f2C = br.ReadInt32();
			bitMap._f30 = br.ReadInt32();
			// получаем тип текстуры. 0x000000FF
			bitMap.m_dwTextureType = bitMap.m_pPixelData << 26;
			bitMap.m_dwTextureType = bitMap.m_dwTextureType >> 26;
			// получаем оффсет. Вместо этого можно просто использовать маску 0x0FFFFF00
			bitMap.m_pPixelData = bitMap.m_pPixelData >> 8;
			bitMap.m_pPixelData = bitMap.m_pPixelData << 16;
			bitMap.m_pPixelData = bitMap.m_pPixelData >> 8;
			bitMap.m_pPixelData += (uint)(FlagInfo.BaseResourceSizeV);
			return bitMap;
		}
		public static FragmentDictionary FragmentDictionary(EndianBinaryReader br)
		{
			FragmentDictionary dict = new FragmentDictionary();
			dict._vmt = br.ReadUInt32();
			dict.m_pBlockMapAdress = br.ReadOffset();
			dict.m_pDrawable = br.ReadOffset();
			dict.m_pTextureDictionary = br.ReadOffset();
			return dict;
		}
	}

}
