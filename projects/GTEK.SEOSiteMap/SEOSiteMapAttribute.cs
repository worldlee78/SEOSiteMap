using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GTEK.SEOSiteMap
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public abstract class SEOSiteMapAttribute : Attribute
	{
		#region Constants

		/// <summary>
		/// The Default Sitemap Prority
		/// </summary>
		public const double DEFAULT_PRIORITY = 0.5;

		#endregion

		#region Fields

		/// <summary>
		/// The priority
		/// </summary>
		protected double priority = DEFAULT_PRIORITY;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the change frequency.
		/// </summary>
		/// <value>The change frequency.</value>
		public SEOSiteMapFrequency ChangeFrequency { get; set; }

		/// <summary>
		/// Gets or sets the priority between 0.0 and 1.0
		/// </summary>
		/// <value>The priority.</value>
		public double Priority
		{
			get { return priority; }
			set
			{
				if (value >= 0.0 && value <= 1.0)
				{
					priority = Math.Round(value, 1);
				}
				else
				{
					priority = DEFAULT_PRIORITY;
				}
			}
		}

		/// <summary>
		/// Gets or sets the last modified date.
		/// </summary>
		/// <value>The last modified.</value>
		public DateTime? LastModified { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="SEOSiteMapAttribute"/> class.
		/// </summary>
		public SEOSiteMapAttribute()
		{
			ChangeFrequency = SEOSiteMapFrequency.Monthly;
			LastModified = null;
		}

		#endregion
	}
}
