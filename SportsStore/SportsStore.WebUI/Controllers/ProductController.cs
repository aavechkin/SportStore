using SportsStore.Domain.Abstract;
using System.Linq;
using System.Web.Mvc;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.ViewModels;

namespace SportsStore.WebUI.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductRepository _repository;

		public int PageSize = 4;

		public ProductController(IProductRepository productRepository)
		{
			_repository = productRepository;
		}

		public ViewResult List(string category, int page = 1)
		{
			var viewModel = new ProductsListViewModel
			{
				Products = _repository.Products
					.Where(p => p.Category == null || p.Category == category)
					.OrderBy(p => p.ProductID)
					.Skip((page - 1) * PageSize)
					.Take(PageSize),
				PagingInfo = new PagingInfo
				{
					CurrentPage = page,
					ItemsPerPage = PageSize,
					TotalItems = _repository.Products.Count()
				},
				CurrentCategory = category
			};

			return View(viewModel);
		}
	}
}
