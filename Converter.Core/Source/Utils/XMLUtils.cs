using System.Collections.Generic;
using System.Xml.Linq;

namespace Converter.Core.Utils
{
	public static class XMLUtils
	{
		public static Dictionary<string, string> GetAttributes(this IEnumerable<XAttribute> attributes)
		{
			// return variable
			Dictionary<string, string> outputDictionary = new Dictionary<string, string>();

			// fill the dictionary
			foreach (XAttribute nodeAttribute in attributes)
			{
				// get the name
				string nodeAttributeName = nodeAttribute.Name.LocalName;

				// get the value
				string nodeAttributeValue = nodeAttribute.Value;

				// add to dictionary
				if (!outputDictionary.ContainsKey(nodeAttributeName))
				{
					outputDictionary.Add(nodeAttributeName, nodeAttributeValue);
				}
			}

			return outputDictionary;
		}
	}
}
