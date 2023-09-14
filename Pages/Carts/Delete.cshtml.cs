using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Carts
{
    public class DeleteModel : PageModel
	{
		private readonly ILogger<DeleteModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;

		public DeleteModel(SneakerShopContext context, ILogger<DeleteModel> logger)
		{
			_logger = logger;
			_mapper = new Mapper(context.GetSession());
			_session = context.GetSession();
		}

		[BindProperty]
		public Cart Cart { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(Guid? id, int? quantity)
		{
			if (id == null)
			{
				return NotFound();
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			Cart = _mapper.FirstOrDefault<Cart>("SELECT * FROM cart WHERE id = ?", id);

			if (Cart != null)
			{
				await _mapper.DeleteAsync<Cart>("WHERE id = ? IF EXISTS", id);

				int? previousQuantity = int.Parse(HttpContext.Session.GetString("Cart"));
				int? newQuantity = previousQuantity - quantity;
				HttpContext.Session.SetString("Cart", newQuantity.ToString());
			}
			stopwatch.Stop();
			_logger.LogInformation("Cart Delete Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("./Index");
		}
	}
}
