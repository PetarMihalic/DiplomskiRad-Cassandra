using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Sneakers
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
		public Sneaker Sneaker { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker where id = ?", id);

			if (sneaker == null)
			{
				return NotFound();
			}
			else
			{
				Sneaker = sneaker;
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
			var sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker where id = ?", id);

			if (sneaker != null)
			{
				Sneaker = sneaker;
				await _mapper.DeleteAsync<Sneaker>("WHERE id = ? IF EXISTS", id);
            }
			stopwatch.Stop();
			_logger.LogInformation("Sneaker Delete Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("/Index");
		}
	}
}
