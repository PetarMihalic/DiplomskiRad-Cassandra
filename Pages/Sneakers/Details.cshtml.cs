using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Sneakers
{
    public class DetailsModel : PageModel
	{
		public string errorMessage = "";
		public string successMessage = "";
		private readonly ILogger<DetailsModel> _logger;
		private readonly Mapper _mapper;
		public DetailsModel(SneakerShopContext context, ILogger<DetailsModel> logger)
		{
			_logger = logger;
			_mapper = context.GetMapper();
		}

		[BindProperty(SupportsGet = true)]
		public Sneaker Sneaker { get; set; } = default!;
		[BindProperty(SupportsGet = true)]
		public List<Inventory> Inventories { get; set; }

		public async Task<IActionResult> OnGetAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?", id);
			var inventory = await _mapper.FetchAsync<Inventory>("SELECT * FROM inventory WHERE sneakerID = ? AND quantity > 0 ALLOW FILTERING", id);


			if (sneaker == null && inventory == null)
			{
				return NotFound();
			}
			else
			{
				Inventories = inventory.ToList();
				Sneaker = sneaker;
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Cart Cart = new Cart();

			Guid sneakerID = Guid.Parse(Request.Form["ID"]);
			float size = float.Parse(Request.Form["size"]);
			int quantity = int.Parse(Request.Form["quantity"]);

			var sneakerOld = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?", sneakerID);
			var inventoryOld = await _mapper.FetchAsync<Inventory>("SELECT * FROM inventory WHERE sneakerID = ? ALLOW FILTERING", sneakerID);
			if (sneakerOld == null && inventoryOld == null)
			{
				return NotFound();
			}
			else
			{
				Inventories = inventoryOld.ToList();
				Sneaker = sneakerOld;
			}

			var inventory = await _mapper.FirstOrDefaultAsync<Inventory>("SELECT * FROM inventory WHERE SneakerID = ? AND Size = ? ALLOW FILTERING", sneakerID, size);
			
			if (inventory.Quantity < quantity)
			{
				errorMessage = "Only " + inventory.Quantity + " available, lower quantity to order.";
				stopwatch.Stop();
				_logger.LogInformation("Cart Create (error) Time: {0}", stopwatch.ElapsedMilliseconds);
				return Page();
			}

			Cart.InventoryID = inventory.ID;
			Cart.Quantity = quantity;
			Cart.Inventory = inventory;

			if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
			{
				Cart.SessionID = HttpContext.Session.Id;
			}
			else
			{
				Cart.UserID = Guid.Parse(HttpContext.Session.GetString("UserID"));
			}
			
			await _mapper.InsertAsync(Cart);

			if (errorMessage == "")
			{
				int currentQuantity = int.Parse(HttpContext.Session.GetString("Cart"));
				int newQuantity = currentQuantity + quantity;
				HttpContext.Session.SetString("Cart", newQuantity.ToString());
				successMessage = "Added to cart";
			}
			stopwatch.Stop();
			_logger.LogInformation("Cart Create Time: {0}", stopwatch.ElapsedMilliseconds);
			return Page();
		}
	}
}
