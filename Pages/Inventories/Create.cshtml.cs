using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Inventories
{
    public class CreateModel : PageModel
	{
		private readonly SneakerShopContext _context;
		private readonly ILogger<CreateModel> _logger;
		private readonly Mapper _mapper;

		public CreateModel(SneakerShopContext context, ILogger<CreateModel> logger)
		{
			_context = context;
			_logger = logger;
			_mapper = context.GetMapper();
		}

		public async Task<IActionResult> OnGetAsync()
		{
			List<Sneaker> sneakers = await _context.GetAllSneakers();
			ViewData["Name"] = new SelectList(sneakers, "ID", "Name");
			return Page();
		}

		[BindProperty]
		public Inventory Inventory { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
			if (Inventory == null)
			{
				return Page();
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Inventory.Sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?", Inventory.SneakerID);
			await _mapper.InsertAsync(Inventory);
			stopwatch.Stop();
			_logger.LogInformation("Inventory Create Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("./Index");
		}
	}
}
