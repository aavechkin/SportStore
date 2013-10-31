using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsStore.Domain.Abstract;

namespace SportsStore.WebUI.Controllers
{
	public class NavigationController : Controller
	{
		private readonly IProductRepository _repository;

		public NavigationController(IProductRepository repository)
		{
			_repository = repository;
		}

		public PartialViewResult Menu(string category = null)
		{
			ViewBag.SelectedCategory = category;

			var categories = _repository.Products.Select(p => p.Category)
				.Distinct()
				.OrderBy(c => c);

			return PartialView(categories);
		}
	}
}
