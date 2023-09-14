using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Carts
{
    public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;

		public IndexModel(SneakerShopContext context, ILogger<IndexModel> logger)
		{
			_logger = logger;
			_mapper = new Mapper(context.GetSession());
		}

		public List<CartPreview> listCart { get; set; } = default!;
		public float TotalCost = 0;
		public async Task OnGetAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
			{
				var carts = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE sessionID = ? ALLOW FILTERING", HttpContext.Session.Id);

				listCart = (from Cart cart in carts
							select new CartPreview
							{
								cartID = cart.ID,
								picture1 = "data:image;base64," + Convert.ToBase64String(cart.Inventory.Sneaker.Picture1),
								size = cart.Inventory.Size,
								name = cart.Inventory.Sneaker.Name,
								quantity = cart.Quantity,
								price = cart.Inventory.Sneaker.Price,
								inventoryID = cart.Inventory.ID,
								total = (float)(cart.Quantity * cart.Inventory.Sneaker.Price)
							}).ToList();
			}
			else
			{
				var carts = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE userID = ? ALLOW FILTERING", Guid.Parse(HttpContext.Session.GetString("UserID")));

				listCart = (from Cart cart in carts
							select new CartPreview
							{
								cartID = cart.ID,
								picture1 = "data:image;base64," + Convert.ToBase64String(cart.Inventory.Sneaker.Picture1),
								size = cart.Inventory.Size,
								name = cart.Inventory.Sneaker.Name,
								quantity = cart.Quantity,
								price = cart.Inventory.Sneaker.Price,
								inventoryID = cart.Inventory.ID,
								total = (float)(cart.Quantity * cart.Inventory.Sneaker.Price)
							}).ToList();
			}
			foreach (var item in listCart)
			{
				TotalCost += item.total;
			}
			TotalCost = (float)Math.Round(TotalCost, 2);
			stopwatch.Stop();
			_logger.LogInformation("Cart Index Time: {0}", stopwatch.ElapsedMilliseconds);
		}

		public class CartPreview
		{
			public Guid cartID { get; set; }
			public string picture1 { get; set; }
			public string name { get; set; }
			public float size { get; set; }
			public int quantity { get; set; }
			public decimal price { get; set; }
			public float total { get; set; }
			public Guid inventoryID { get; set; }
		}
	}
}
