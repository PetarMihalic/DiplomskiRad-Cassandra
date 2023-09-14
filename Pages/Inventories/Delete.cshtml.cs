using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Inventories
{
    public class DeleteModel : PageModel
	{
		private readonly ILogger<DeleteModel> _logger;
		private readonly Mapper _mapper;
		public DeleteModel(SneakerShopContext context, ILogger<DeleteModel> logger)
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
			else
			{
				Inventory = inventory;
			}
			return Page();
		}

		public async Task<IActionResult> OnPostAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			var inventory = await _mapper.FirstOrDefaultAsync<Inventory>("SELECT * FROM inventory WHERE id = ?", id);

			if (inventory != null)
			{
				Inventory = inventory;
				await _mapper.DeleteAsync<Inventory>("WHERE id = ? IF EXISTS", id);
            }
			stopwatch.Stop();
			_logger.LogInformation("Inventory Delete Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("./Index");
		}
	}
}
