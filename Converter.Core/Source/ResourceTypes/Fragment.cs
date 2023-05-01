using System.Numerics;

namespace Converter.Core.ResourceTypes
{
	// iv
	public class Fragment
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
		public uint _fC4; // -1
		public uint _fC8;
		public uint m_pChildGroupName;
		public uint _fD0; // pointer to offset to zeroes
		public uint _fD4; // pointer to offset to the unknown section 
		public uint _fD8;
		public uint _fDC;
		public uint _fE0;
	}
}
