
namespace Converter.Core
{
	/// <summary>
	/// Represents a basic structure of magic bytes in any RAGE file
	/// </summary>
	public class RSCHeader
	{
		public byte[] Magic = new byte[3];
		public byte Version;
	}
}
