using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopCassandra.Data;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Orders
{
    public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly Mapper _mapper;
		private readonly SneakerShopContext _context;
		public IndexModel(SneakerShopContext context, ILogger<IndexModel> logger)
		{
			_logger = logger;
			_mapper = context.GetMapper();
			_context = context;
		}
		public IList<Models.Orders> Order { get; set; } = default!;

		[BindProperty]
		public string OrderID { get; set; }
		[BindProperty]
		public string Status { get; set; }

		public async Task OnGetAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Order = await _context.GetAllOrders();
			stopwatch.Stop();
			_logger.LogInformation("Order Index Time: {0}", stopwatch.ElapsedMilliseconds);
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			try
			{
                await _mapper.UpdateAsync<Models.Orders>("SET status = ? WHERE id = ? IF EXISTS", Status, Guid.Parse(OrderID));
            }
			catch (DbUpdateConcurrencyException)
			{
				throw;
			}
			stopwatch.Stop();
			_logger.LogInformation("Order Edit Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("/Orders/Index");
		}
	}
}
