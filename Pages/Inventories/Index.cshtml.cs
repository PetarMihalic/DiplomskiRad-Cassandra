using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Inventories
{
    public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly SneakerShopContext _context;
		public IndexModel(SneakerShopContext context, ILogger<IndexModel> logger)
		{
			_logger = logger;
			_context = context;
		}

		public IList<Inventory> Inventory { get; set; } = default!;

		public async Task OnGetAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Inventory = await _context.GetInventory();
			stopwatch.Stop();
			_logger.LogInformation("Inventory Index Time: {0}", stopwatch.ElapsedMilliseconds);
		}
	}
}
