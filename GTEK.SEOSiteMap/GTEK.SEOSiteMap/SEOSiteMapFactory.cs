using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;
using GTEK.Common.Core;

namespace GTEK.SEOSiteMap
{
	public static class SEOSiteMapFactory
	{
		#region Constants

		private const string SITEMAP_NAMESPACE = "http://www.sitemaps.org/schemas/sitemap/0.9";

		#endregion

		#region Fields

		private static RouteCollection routes = null;
		private static Assembly assembly = null;

		#endregion

		#region Properties

		#endregion

		#region Methods

		public static string Create(RequestContext context)
		{
			// Test to see that we've been registered
			if (routes == null || assembly == null)
				throw new InvalidOperationException("Missing required fields. Have you registered the factory?");

			// Start creating the XML document
			XNamespace ns = SITEMAP_NAMESPACE;
			XDocument doc = new XDocument();
			XElement urlSet = new XElement(ns + "urlset");
			doc.Add(urlSet);

			// Get the UrlHelper
			var urlHelper = new UrlHelper(context, routes);
			
			// Get the static sitemap attributes			
			var controllers = assembly.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.IsPublic && typeof(Controller).IsAssignableFrom(x));

			foreach(var controller in controllers)
			{
				// Get all actions that have SEO attributes
				var actions = controller.GetMethods().Where(x => x.IsPublic && !x.IsAbstract && x.ReturnType != null && typeof(ActionResult).IsAssignableFrom(x.ReturnType) && (x.GetCustomAttributes(true).Any(y => y is SEOSiteMapAttribute)));
				
				foreach(var action in actions)
				{
					var siteMapAttribute = action.GetCustomAttributes(typeof(SEOSiteMapAttribute), true).FirstOrDefault();

					// Shouldn't happen, but just in case
					if(siteMapAttribute == null)
						continue;

					if(siteMapAttribute is SEOStaticSiteMapAttribute)
					{
						var node = siteMapAttribute as SEOStaticSiteMapAttribute;

						// If the node hasn't been filled out, let's try and determine this on our own.
						// We can't infer the Area through reflection, so we assume there isn't one if it wasn't listed
						if (node.Route.IsNullOrEmpty() && node.Controller.IsNullOrEmpty() && node.Area.IsNullOrEmpty())
						{
							node.Action = action.Name;
							node.Controller = controller.Name.Replace("Controller", "");
						}
						
						// Final check to ensure we didn't miss anything
						if(!node.Route.IsNullOrEmpty() && !(node.Action.IsNullOrEmpty() || node.Controller.IsNullOrEmpty() || node.Area.IsNullOrEmpty()))
						{
							throw new InvalidOperationException("Cannot have both route and either Area, Controller or Action be not null.");
						}
						else if (node.Route.IsNullOrEmpty() && (node.Action.IsNullOrEmpty() || node.Controller.IsNullOrEmpty()))
						{
							throw new InvalidOperationException("Area and Controller are required fields if no specified route.");
						}

						// Get the location
						string loc = null;

						if (node.Route.IsNullOrEmpty())
						{
							loc = urlHelper.Action(node.Action, node.Controller, new { area = node.Area.IsNullOrEmpty() ? null : node.Area }, "http");	
						}
						else
						{
							loc = urlHelper.RouteUrl(node.Route, null, "http");
						}

						XElement url = new XElement(ns + "url",
							new XElement(ns + "loc", loc),
							new XElement(ns + "changefreq", node.ChangeFrequency.ToString().ToLower()),
							new XElement(ns + "priority", node.Priority));

						if(node.LastModified.HasValue)
						{
							url.Add(new XElement("lastmod", node.LastModified.Value.ToString("yyyy-dd-MM")));
						}

						urlSet.Add(url);
					}
					else if (siteMapAttribute is SEODynamicSiteMapAttribute)
					{
						var node = siteMapAttribute as SEODynamicSiteMapAttribute;

						// Check that the sitemap node is valid
						Guard.ArgumentNotNullOrEmptyString(node.Route, "Route");
						Guard.ArgumentNotNullOrEmptyString(node.KeyToken, "KeyToken");
						Guard.ArgumentNotNull(node.DynamicNodeProvider, "DynamicNodeProvider");

						// Get the location
						var route = urlHelper.RouteCollection[node.Route];
						if (route == null)
							throw new Exception("Could not locate route named {0}. Cannot create sitemap.".FormatWith(node.Route));

						// Get repository
						var dynamicNodeProviderType = assembly.GetTypes().FirstOrDefault(x => x.Name == node.DynamicNodeProvider);
						if (dynamicNodeProviderType == null)
							throw new Exception("Could not locate dynamic node provider named {0}. Cannot create sitemap.".FormatWith(node.DynamicNodeProvider));

						var dynamicNodeProvider = Activator.CreateInstance(dynamicNodeProviderType) as IDynamicNodeProvider;
						var results = dynamicNodeProvider.PossibleValues();

						foreach (var result in results)
						{
							
							// Get the location
							string loc = urlHelper.RouteUrl(node.Route, new RouteValueDictionary() { { node.KeyToken, result } }, "http", urlHelper.RequestContext.HttpContext.Request.Url.Host);

							XElement url = new XElement(ns + "url",
								new XElement(ns + "loc", loc),
								new XElement(ns + "changefreq", node.ChangeFrequency.ToString().ToLower()),
								new XElement(ns + "priority", node.Priority));

							if (node.LastModified.HasValue)
							{
								url.Add(new XElement(ns + "lastmod", node.LastModified.Value.ToString("yyyy-dd-MM")));
							}

							urlSet.Add(url);
						}
					}
				}
			}

			var output = new MemoryStream();
			using (XmlWriter writer = XmlWriter.Create(output, new XmlWriterSettings() { Indent = true, Encoding = new UTF8Encoding() }))
			{
				doc.WriteTo(writer);
			}

			output.Seek(0L, SeekOrigin.Begin);

			return new StreamReader(output).ReadToEnd();
		}

		public static void Register(RouteCollection routes, Assembly assembly)
		{
			SEOSiteMapFactory.assembly = assembly;
			SEOSiteMapFactory.routes = routes;
		}

		#endregion
	}
}
