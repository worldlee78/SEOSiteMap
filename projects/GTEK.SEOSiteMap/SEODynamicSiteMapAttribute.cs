using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTEK.SEOSiteMap
{
	public class SEODynamicSiteMapAttribute : SEOSiteMapAttribute
	{
		#region Dynamic Sitemap Properties

		public string DynamicNodeProvider { get; set; }
		public string Route { get; set; }
		public string KeyToken { get; set; }

		#endregion
	}
}
