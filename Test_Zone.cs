using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Converter
{
	internal class Test_Zone
	{
		public static void LOADDDS()
		{
			string ddsPath = "C:\\Users\\im\\Downloads\\smic_player_default_im_texture\\wrinklemap2_n1.dds";
			SharpDX.Toolkit.Graphics.Image textureDDS = SharpDX.Toolkit.Graphics.Image.Load(ddsPath);
			textureDDS.Description.Format = SharpDX.DXGI.Format.R16G16B16A16_UInt;
			Console.WriteLine("loaded");
        }
	}
}
