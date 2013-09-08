using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTEK.SEOSiteMap
{
	public interface IDynamicNodeProvider
	{
		string[] PossibleValues();
	}
}
