using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ninject.Planning.Targets;
using SportsStore.Domain.Abstract;
using SportsStore.Domain.Entities;
using SportsStore.WebUI.Controllers;
using SportsStore.WebUI.HtmlHelpers;
using SportsStore.WebUI.Models;
using SportsStore.WebUI.ViewModels;

namespace SportsStore.UnitTests
{
	[TestClass]
	public class ProductControllerTest
	{
		[TestMethod]
		public void Can_Paginate()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new[]
				{
					new Product { ProductID = 1, Name = "P1" }, 
					new Product { ProductID = 2, Name = "P2" },
					new Product { ProductID = 3, Name = "P3" },
					new Product { ProductID = 4, Name = "P4" },
					new Product { ProductID = 5, Name = "P5" }
				}.AsQueryable());

			var controller = new ProductController(mock.Object) { PageSize = 3 };

			var result = (ProductsListViewModel)controller.List(null, 2).Model;

			var prodArray = result.Products.ToArray();
			Assert.IsTrue(prodArray.Length == 2);
			Assert.AreEqual(prodArray[0].Name, "P4");
			Assert.AreEqual(prodArray[1].Name, "P5");
		}

		[TestMethod]
		public void Can_Generate_Page_Links()
		{
			HtmlHelper helper = null;

			var pagingInfo = new PagingInfo
			{
				CurrentPage = 2,
				TotalItems = 28,
				ItemsPerPage = 10
			};

			Func<int, string> pageUrlDelegate = i => "Page" + i;

			var result = helper.PageLinks(pagingInfo, pageUrlDelegate);

			Assert.AreEqual(result.ToString(), @"<a href=""Page1"">1</a><a class=""selected"" href=""Page2"">2</a><a href=""Page3"">3</a>");
		}

		[TestMethod]
		public void Can_Send_Pagination_View_Model()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new[]
				{
					new Product { ProductID = 1, Name = "P1" }, 
					new Product { ProductID = 2, Name = "P2" },
					new Product { ProductID = 3, Name = "P3" },
					new Product { ProductID = 4, Name = "P4" },
					new Product { ProductID = 5, Name = "P5" }
				}.AsQueryable());

			var controller = new ProductController(mock.Object) { PageSize = 3 };

			var result = (ProductsListViewModel)controller.List(null, 2).Model;

			var pagingInfo = result.PagingInfo;
			Assert.AreEqual(pagingInfo.CurrentPage, 2);
			Assert.AreEqual(pagingInfo.ItemsPerPage, 3);
			Assert.AreEqual(pagingInfo.TotalItems, 5);
			Assert.AreEqual(pagingInfo.TotalPages, 2);
		}

		[TestMethod]
		public void Can_Filter_Products()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new[]
				{
					new Product { ProductID = 1, Name = "P1", Category = "Cat1" }, 
					new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
					new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
					new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
					new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
				}.AsQueryable());

			var controller = new ProductController(mock.Object) { PageSize = 3 };

			var result = ((ProductsListViewModel)controller.List("Cat2", 1).Model).Products.ToArray();

			Assert.AreEqual(result.Length, 2);
			Assert.IsTrue(result[0].Name == "P2" && result[0].Category == "Cat2");
			Assert.IsTrue(result[1].Name == "P4" && result[1].Category == "Cat2");
		}

		[TestMethod]
		public void Can_Create_Categories()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new[]
				{
					new Product { ProductID = 1, Name = "P1", Category = "Apple" }, 
					new Product { ProductID = 2, Name = "P2", Category = "Apple" },
					new Product { ProductID = 3, Name = "P3", Category = "Zed" },
					new Product { ProductID = 4, Name = "P4", Category = "Computer" },
				}.AsQueryable());

			var target = new NavigationController(mock.Object);
			var results = ((IEnumerable<string>)target.Menu().Model).ToArray();

			Assert.AreEqual(results.Length, 3);
			Assert.AreEqual(results[0], "Apple");
			Assert.AreEqual(results[1], "Computer");
			Assert.AreEqual(results[2], "Zed");
		}

		[TestMethod]
		public void Can_Use_View_Bag()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new[]
			{
				new Product { ProductID = 1, Name = "P1", Category = "Apple" }, 
				new Product { ProductID = 4, Name = "P2", Category = "Computer" }
			}.AsQueryable());

			const string categoryToTest = "Apple";

			var target = new NavigationController(mock.Object);
			var result = target.Menu(categoryToTest).ViewBag.SelectedCategory;

			Assert.AreEqual(categoryToTest, result);
		}

		[TestMethod]
		public void Generate_Category_Specific_Product_Count()
		{
			var mock = new Mock<IProductRepository>();
			mock.Setup(m => m.Products).Returns(new Product[] {
				new Product {ProductID = 1, Name = "P1", Category = "Cat1"},
				new Product {ProductID = 2, Name = "P2", Category = "Cat2"},
				new Product {ProductID = 3, Name = "P3", Category = "Cat1"},
				new Product {ProductID = 4, Name = "P4", Category = "Cat2"},
				new Product {ProductID = 5, Name = "P5", Category = "Cat3"}
			}.AsQueryable());

			var target = new ProductController(mock.Object) { PageSize = 3 };

			var res1 = ((ProductsListViewModel)target.List("Cat1").Model).PagingInfo.TotalItems;
			var res2 = ((ProductsListViewModel)target.List("Cat2").Model).PagingInfo.TotalItems;
			var res3 = ((ProductsListViewModel)target.List("Cat3").Model).PagingInfo.TotalItems;
			var resAll = ((ProductsListViewModel)target.List(null).Model).PagingInfo.TotalItems;

			Assert.AreEqual(res1, 2);
			Assert.AreEqual(res2, 2);
			Assert.AreEqual(res3, 1);
			Assert.AreEqual(resAll, 5);
		}
	}
}
