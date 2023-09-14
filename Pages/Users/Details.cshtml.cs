using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
    public class DetailsModel : PageModel
	{
		private readonly ILogger<DetailsModel> _logger;
		private readonly Mapper _mapper;
		public DetailsModel(SneakerShopContext context, ILogger<DetailsModel> logger)
		{
			_logger = logger;
			_mapper = context.GetMapper();
		}

		public User User { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var user = await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE id = ?", id);
			if (user == null)
			{
				return NotFound();
			}
			else
			{
				User = user;
			}
			stopwatch.Stop();
			_logger.LogInformation("User Details Time: {0}", stopwatch.ElapsedMilliseconds);
			return Page();
		}

		public void OnPost()
		{
			try
			{
				HttpContext.Session.Clear();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return;
			}
			Response.Redirect("/Index");
		}
	}
}
