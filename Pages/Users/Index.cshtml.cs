using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
    public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly SneakerShopContext _context;
		public IndexModel(SneakerShopContext context, ILogger<IndexModel> logger)
		{
			_logger = logger;
			_session = context.GetSession();
			_context = context;
		}

		public IList<User> User { get; set; } = default!;

		public async Task OnGetAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			User = await _context.GetAllUsers();
			stopwatch.Stop();
			_logger.LogInformation("User Index Time: {0}", stopwatch.ElapsedMilliseconds);
			
		}
	}
}
