# [SEO Site Map Generator v0.1.0]

I noticed a surprising lack of SEO Site Map generation tools for the MVC framework, so I decided to code my own and share it with the community.

## To Use:

For each Action that you wish to include in the SiteMap that displays only static content (the content does not change based on incoming parameters) use the [SEOStaticSiteMapAttribute].

	[HttpGet]
	[SEOStaticSiteMap]
	public ActionResult Index()
	{
		return View();
	}
 
For Actions that can change relative to the incoming parameters you need to create a DynamicNodeProvider, select a KeyToken, and pick a Route that will serve up the information.

	[HttpGet]
	[SEODynamicSiteMap(Route = "CatalogBrowse", DynamicNodeProvider = "CatalogNodeProvider", KeyToken = "categoryId")]
	public ActionResult Results(int categoryId, int orderBy = 0, int page = 0)
	{
		// Do lookup of items in the catalog for that categoryId
		var products = catalog.GetProductsByCategoryId(categoryId);

		return View(products);
	}