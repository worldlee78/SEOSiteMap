using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTEK.SEOSiteMap
{
	public abstract class SEODynamicNodeProvider : IDynamicNodeProvider
	{
		public string[] PossibleValues()
		{
			return new string[0];
		}
	}
}
