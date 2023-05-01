using System;
using System.IO;
using Converter.Core.Utils;

namespace Converter.Core.Utils
{
	/*
	 * This class is written using these articles:
	 * 
	 * https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dds-header
	 * https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dds-pixelformat
	 * https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide
	 * https://learn.microsoft.com/en-us/windows/win32/api/dxgiformat/ne-dxgiformat-dxgi_format
	 * 
	 * This implemenation doesn't include the DX10-specific header because it never used here.
	 */

	/// <summary>
	/// Implements a custom class for working with DDS file format
	/// </summary>
	public static class DDS
	{
		/// <summary>
		/// Write DDS data (like header and pixel data) to the output file
		/// </summary>
		/// <param name="hdr">DDS Header</param>
		/// <param name="path">Full path to the output file</param>
		/// <param name="pixelData">An array contains pixel data of your image</param>
		/// <param name="overwrite">Should the output file be overwritten if it's already exists (true - yes, false - no)</param>
		public static void Write(this DDS_HEADER hdr, string path, byte[] pixelData, bool overwrite)
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path", "You did not provide the output path.");
			}

			if (pixelData == null)
			{
				throw new ArgumentNullException("pixelData", "You did not provide the pixel data.");
			}

			if (pixelData.Length == 0)
			{
				throw new ArgumentException("Nothing to write!");
			}

			if (File.Exists(path))
			{
				if (overwrite)
				{
					Console.WriteLine($"[WARNING] output file \"{Path.GetFileName(path)}\" is already exists, it will be overwitten.");
				}
				else
				{
					string uniquePath = FileUtils.EnsureUnique(path);
					path = uniquePath;
					Console.WriteLine($"[WARNING] output file \"{Path.GetFileName(path)}\" is already exists, saving as \"{Path.GetFileName(uniquePath)}\".");
				}
			}

			using (BinaryWriter bw = new BinaryWriter(File.Create(path)))
			{
				bw.Write(hdr.dwMagic);
				bw.Write(hdr.dwSize);
				bw.Write(hdr.dwFlags);
				bw.Write(hdr.dwHeight);
				bw.Write(hdr.dwWidth);
				bw.Write(hdr.dwPitchOrLinearSize);
				bw.Write(hdr.dwDepth);
				bw.Write(hdr.dwMipMapCount);

				for (int i = 0; i < hdr.dwReserved1.Length; i++)
				{
					bw.Write(hdr.dwReserved1[i]);
				}
				
				bw.Write(hdr.ddspf.dwSize);
				bw.Write(hdr.ddspf.dwFlags);
				bw.Write(hdr.ddspf.dwFourCC);
				bw.Write(hdr.ddspf.dwRGBBitCount);
				bw.Write(hdr.ddspf.dwRBitMask);
				bw.Write(hdr.ddspf.dwGBitMask);
				bw.Write(hdr.ddspf.dwBBitMask);
				bw.Write(hdr.ddspf.dwABitMask);

				bw.Write(hdr.dwCaps);
				bw.Write(hdr.dwCaps2);
				bw.Write(hdr.dwCaps3);
				bw.Write(hdr.dwCaps4);
				bw.Write(hdr.dwReserved2);

				bw.Write(pixelData);
			}
		}
	}

	/// <summary>
	/// Describes a DDS file header.
	/// </summary>
	public class DDS_HEADER
	{
		/// <summary>
		/// The four character code value 'DDS ' (0x20534444)
		/// </summary>
		public readonly uint dwMagic = 0x20534444u;

		/// <summary>
		/// Size of structure. This member must be set to 124.
		/// </summary>
		public readonly uint dwSize = 0x7Cu;

		/// <summary>
		/// Flags to indicate which members contain valid data.
		/// </summary>
		public uint dwFlags;

		/// <summary>
		/// Surface height (in pixels).
		/// </summary>
		public uint dwHeight;

		/// <summary>
		/// Surface width (in pixels).
		/// </summary>
		public uint dwWidth;

		/// <summary>
		/// The pitch or number of bytes per scan line in an uncompressed texture;<br />
		/// the total number of bytes in the top level texture for a compressed texture.<br />
		/// For information about how to compute the pitch, see the DDS File Layout section of the <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dx-graphics-dds-pguide">Programming Guide for DDS</a>.
		/// </summary>
		public uint dwPitchOrLinearSize;

		/// <summary>
		/// Depth of a volume texture (in pixels), otherwise unused.
		/// </summary>
		public uint dwDepth;

		/// <summary>
		/// Number of mipmap levels, otherwise unused.
		/// </summary>
		public uint dwMipMapCount;

		/// <summary>
		/// Unused.
		/// </summary>
		public readonly uint[] dwReserved1 = new uint[11];

		/// <summary>
		/// The pixel format (see <a href="https://learn.microsoft.com/en-us/windows/win32/direct3ddds/dds-pixelformat">DDS_PIXELFORMAT</a>).
		/// </summary>
		public DDS_PIXELFORMAT ddspf = new DDS_PIXELFORMAT();

		/// <summary>
		/// Specifies the complexity of the surfaces stored.
		/// </summary>
		public uint dwCaps;

		/// <summary>
		/// Additional detail about the surfaces stored.
		/// </summary>
		public readonly uint dwCaps2;

		/// <summary>
		/// Unused.
		/// </summary>
		public readonly uint dwCaps3;

		/// <summary>
		/// Unused.
		/// </summary>
		public readonly uint dwCaps4;

		/// <summary>
		/// Unused.
		/// </summary>
		public readonly uint dwReserved2;
	}

	/// <summary>
	/// Describes a surface pixel format.
	/// </summary>
	public class DDS_PIXELFORMAT
	{
		/// <summary>
		/// Structure size; set to 32 (bytes).
		/// </summary>
		public readonly uint dwSize = 0x20u;

		/// <summary>
		/// Values which indicate what type of data is in the surface.
		/// </summary>
		public uint dwFlags;

		/// <summary>
		/// Four-character codes for specifying compressed or custom formats.<br />
		/// Possible values include: <em>DXT1, DXT2, DXT3, DXT4</em>, or <em>DXT5</em>.<br />
		/// When using a four-character code, dwFlags must include <em>DDPF_FOURCC.</em><br />
		/// </summary>
		public uint dwFourCC;

		/// <summary>
		/// Number of bits in an RGB (possibly including alpha) format.
		/// Valid when <strong>dwFlags</strong> includes <em>DDPF_RGB, DDPF_LUMINANCE</em>, or <em>DDPF_YUV</em>.
		/// </summary>
		public uint dwRGBBitCount;

		/// <summary>
		/// Red (or luminance or Y) mask for reading color data.<br />
		/// For instance, given the A8R8G8B8 format, the red mask would be 0x00ff0000.
		/// </summary>
		public uint dwRBitMask;

		/// <summary>
		/// Green (or U) mask for reading color data.<br />
		/// For instance, given the A8R8G8B8 format, the green mask would be 0x0000ff00.
		/// </summary>
		public uint dwGBitMask;

		/// <summary>
		/// Blue (or V) mask for reading color data.<br />
		/// For instance, given the A8R8G8B8 format, the blue mask would be 0x000000ff.
		/// </summary>
		public uint dwBBitMask;

		/// <summary>
		/// Alpha mask for reading alpha data.<br />
		/// dwFlags must include <em>DDPF_ALPHAPIXELS</em> or <em>DDPF_ALPHA</em>.<br />
		/// For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
		/// </summary>
		public uint dwABitMask;

		#region FourCC constants
		public readonly uint DXT1 = MakeFourCC('D', 'X', 'T', '1');
		public readonly uint DXT2 = MakeFourCC('D', 'X', 'T', '2');
		public readonly uint DXT3 = MakeFourCC('D', 'X', 'T', '3');
		public readonly uint DXT4 = MakeFourCC('D', 'X', 'T', '4');
		public readonly uint DXT5 = MakeFourCC('D', 'X', 'T', '5');

		private static uint MakeFourCC(char c0, char c1, char c2, char c3)
		{
			uint result = c0;
			result |= (uint)c1 << 8;
			result |= (uint)c2 << 16;
			result |= (uint)c3 << 24;
			return result;
		}
		#endregion
	}

	/// <summary>
	/// Flags to indicate which members contain valid data.
	/// </summary>
	public enum DDSD : uint
	{
		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		CAPS = 0x1,

		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		HEIGHT = 0x2,

		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		WIDTH = 0x4,

		/// <summary>
		/// Required when pitch is provided for an uncompressed texture.
		/// </summary>
		PITCH = 0x8,

		/// <summary>
		/// Required in every .dds file.
		/// </summary>
		PIXELFORMAT = 0x1000,

		/// <summary>
		/// Required in a mipmapped texture.
		/// </summary>
		MIPMAPCOUNT = 0x20000,

		/// <summary>
		/// Required when pitch is provided for a compressed texture.
		/// </summary>
		LINEARSIZE = 0x80000,

		/// <summary>
		/// Required in a depth texture.
		/// </summary>
		DEPTH = 0x800000
	}

	/// <summary>
	/// Values which indicate what type of data is in the surface.
	/// </summary>
	public enum DDPF : uint
	{
		/// <summary>
		/// Texture contains alpha data; <strong>dwRGBAlphaBitMask</strong> contains valid data.
		/// </summary>
		ALPHAPIXELS = 0x1,

		/// <summary>
		/// Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
		/// </summary>
		ALPHA = 0x2,

		/// <summary>
		/// Texture contains compressed RGB data; <strong>dwFourCC</strong> contains valid data.
		/// </summary>
		FOURCC = 0x4,

		/// <summary>
		/// Texture contains uncompressed RGB data; <strong>dwRGBBitCount</strong> and the RGB masks (<strong>dwRBitMask</strong>, <strong>dwGBitMask</strong>, <strong>dwBBitMask</strong>) contain valid data.
		/// </summary>
		RGB = 0x40,

		/// <summary>
		/// Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)	
		/// </summary>
		YUV = 0x200,

		/// <summary>
		/// Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
		/// </summary>
		LUMINANCE = 0x20000
	}

	/// <summary>
	/// Specifies the complexity of the surfaces stored.
	/// </summary>
	public enum DDSCAPS : uint
	{
		/// <summary>
		/// Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
		/// </summary>
		COMPLEX = 0x8,

		/// <summary>
		/// Optional; should be used for a mipmap.
		/// </summary>
		MIPMAP = 0x400000,

		/// <summary>
		/// Required
		/// </summary>
		TEXTURE = 0x1000
	}

	/// <summary>
	/// Additional detail about the surfaces stored.
	/// </summary>
	public enum DDSCAPS2 : uint
	{
		/// <summary>
		/// Required for a cube map.
		/// </summary>
		CUBEMAP = 0x200,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_POSITIVEX = 0x400,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_NEGATIVEX = 0x800,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_POSITIVEY = 0x1000,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_NEGATIVEY = 0x2000,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_POSITIVEZ = 0x4000,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		CUBEMAP_NEGATIVEZ = 0x8000,

		/// <summary>
		/// Required when these surfaces are stored in a cube map.
		/// </summary>
		VOLUME = 0x200000
	}
}
