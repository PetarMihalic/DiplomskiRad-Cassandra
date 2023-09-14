using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Orders
{
    public class MyOrdersModel : PageModel
	{
		private readonly ILogger<MyOrdersModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public MyOrdersModel(SneakerShopContext context, ILogger<MyOrdersModel> logger)
		{
			_logger = logger;
			_session = context.GetSession();
			_mapper = context.GetMapper();
		}

		public IList<Models.Orders> Order { get; set; }

		public async Task OnGetAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Guid UserID = Guid.Parse(HttpContext.Session.GetString("UserID"));
			var order = await _mapper.FetchAsync<Models.Orders>("SELECT * FROM orders where UserID = ? ALLOW FILTERING", UserID);
			Order = order.ToList();
			stopwatch.Stop();
			_logger.LogInformation("My Orders Time: {0}", stopwatch.ElapsedMilliseconds);
		}
	}
}
