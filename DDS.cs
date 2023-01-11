using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class DDS
	{
		public static bool BuildDDS(byte[] pixelData, int width, int height, int format, uint textureSize, string basicName)
		{
			BinaryWriter dataOut = new BinaryWriter(new FileStream(basicName, FileMode.Create));

			uint ddsFlags = 528391;
			uint ddsMagic = 542327876;

			dataOut.Write(ddsMagic);
			dataOut.Write(124);
			dataOut.Write(ddsFlags);
			dataOut.Write(height);
			dataOut.Write(width);

			dataOut.Write(textureSize);
			dataOut.Write(0);  // volume
			dataOut.Write(0);  // mipmaps
			dataOut.Write(0);  // reversed!?

			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0x20);


			dataOut.Write(0x4);

			if (format == 18) dataOut.Write(827611204);// dxt1
			else if (format == 19) dataOut.Write(861165636);// dxt3
			else if (format == 20) dataOut.Write(894720068);// dxt5
			else if (format == 6) dataOut.Write(943208504);// 8888
			else dataOut.Write(0);

			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(textureSize);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);
			dataOut.Write(0);

			dataOut.Write(pixelData);
			dataOut.Close();


			return true;
		}
	}
}
