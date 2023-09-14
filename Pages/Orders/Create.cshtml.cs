using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;
using static SneakerShopCassandra.Pages.Carts.IndexModel;

namespace SneakerShopCassandra.Pages.Orders
{
    public class CreateModel : PageModel
	{
		private readonly ILogger<CreateModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public CreateModel(SneakerShopContext context, ILogger<CreateModel> logger)
		{
			_logger = logger;
			_session = context.GetSession();
			_mapper = context.GetMapper();
		}

		[BindProperty(SupportsGet = true)]
		public User User { get; set; } = default!;

		[BindProperty(SupportsGet = true)]
		public string PaymentType { get; set; } = default!;

		[BindProperty(SupportsGet = true)]
		public Models.Orders Order { get; set; } = default!;

		public List<CartPreview> listCart { get; set; } = default!;
		public float TotalCost = 0;
		public async Task OnGetAsync()
		{
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
				User = await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE ID = ? ALLOW FILTERING", Guid.Parse(HttpContext.Session.GetString("UserID")));
				
                var carts = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE userID = ? ALLOW FILTERING", User.ID);

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
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Models.Orders Order = new Models.Orders();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("UserID"))) Order.UserID = User.ID;
			Random random = new Random();
			Order.ID = Guid.NewGuid();
			Order.Name = "ORDER-" + DateTime.Now.ToString() + "-" + random.Next(100, 1000);
			Order.PaymentType = PaymentType;
			Order.CreatedDate = DateTime.Now;

            if (User.ID.ToString() == "")
			{
				var carts = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE sessionID = ? ALLOW FILTERING", HttpContext.Session.Id);

				listCart = (from Cart cart in carts
							select new CartPreview
							{
								quantity = cart.Quantity,
								inventoryID = cart.InventoryID
							}).ToList();
			}
			else
			{
				var carts = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE userID = ? ALLOW FILTERING", User.ID);

				listCart = (from Cart cart in carts
							select new CartPreview
							{
								quantity = cart.Quantity,
								inventoryID = cart.InventoryID
							}).ToList();
			}
			List<OrderDetails> orderDetailsList = new List<OrderDetails>();
			foreach (CartPreview cartPreview in listCart)
			{
				OrderDetails OrderDetails = new OrderDetails();
				OrderDetails.OrderID = Order.ID;
				OrderDetails.InventoryID = cartPreview.inventoryID;
				OrderDetails.Quantity = cartPreview.quantity;
				OrderDetails.Inventory = await _mapper.FirstOrDefaultAsync<Inventory>("SELECT * FROM inventory WHERE id = ?", cartPreview.inventoryID);
                Inventory inventory = OrderDetails.Inventory;
                inventory.Quantity = inventory.Quantity - cartPreview.quantity;
                if (inventory.Quantity == 0)
                {
                    var cartItems = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE inventoryID = ? ALLOW FILTERING", inventory.ID);
                    foreach (Cart cart in cartItems)
                    {
                        await _mapper.DeleteAsync<Cart>("WHERE id = ? IF EXISTS", cart.ID);
                    }
                }
                await _mapper.UpdateAsync<Inventory>("SET quantity = ? WHERE id = ? IF EXISTS", inventory.Quantity, inventory.ID);
                orderDetailsList.Add(OrderDetails);
				
				await _mapper.InsertAsync(OrderDetails);
			}
			Order.OrderDetails = orderDetailsList;
            await _mapper.InsertAsync(Order);

            if (User.ID.ToString() != "")
			{
				var cartItems = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE userID = ? ALLOW FILTERING", User.ID);
				foreach (Cart cart in cartItems)
				{
                    await _mapper.DeleteAsync<Cart>("WHERE id = ? IF EXISTS", cart.ID);
                }
				HttpContext.Session.SetString("Cart", "0");
			}
			else
			{
				var cartItems = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE sessionID = ? ALLOW FILTERING", HttpContext.Session.Id);
				foreach (Cart cart in cartItems)
				{
                    await _mapper.DeleteAsync<Cart>("WHERE id = ? IF EXISTS", cart.ID);
                }
				HttpContext.Session.SetString("Cart", "0");
			}
			stopwatch.Stop();
			_logger.LogInformation("Order Create Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("/Index");
		}
	}
}
