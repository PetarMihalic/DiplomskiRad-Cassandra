using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Inventories
{
    public class EditModel : PageModel
	{
		private readonly ILogger<EditModel> _logger;
		private readonly Mapper _mapper;
		public EditModel(SneakerShopContext context, ILogger<EditModel> logger)
		{
			_logger = logger;
			_mapper = context.GetMapper();
		}

		[BindProperty]
		public Inventory Inventory { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var inventory = await _mapper.FirstOrDefaultAsync<Inventory>("SELECT * FROM inventory WHERE id = ?", id);
			if (inventory == null)
			{
				return NotFound();
			}
			Inventory = inventory;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			try
			{
				await _mapper.UpdateAsync<Inventory>("SET quantity = ? WHERE id = ? IF EXISTS",Inventory.Quantity, Inventory.ID);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await InventoryExistsAsync(Inventory.ID))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			stopwatch.Stop();
			_logger.LogInformation("Inventory Edit Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("./Index");
		}

		private async Task<bool> InventoryExistsAsync(Guid id)
		{
			var inv = await _mapper.FirstOrDefaultAsync<Inventory>("SELECT * FROM inventory WHERE id = ?", id);
			return (inv != null);
		}
	}
}
