using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTEK.SEOSiteMap
{
	public class SEOStaticSiteMapAttribute : SEOSiteMapAttribute
	{
		#region Sitemap Location Properties

		public string Route { get; set; }
		public string Area { get; set; }
		public string Controller { get; set; }
		public string Action { get; set; }

		#endregion
	}
}
