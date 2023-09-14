using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
    public class DeleteModel : PageModel
	{
		private readonly ILogger<DeleteModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public DeleteModel(SneakerShopContext context, ILogger<DeleteModel> logger)
		{
			_logger = logger;
			_session = context.GetSession();
			_mapper = context.GetMapper();
		}

		[BindProperty]
		public User User { get; set; } = default!;

		public async Task<IActionResult> OnGetAsync(Guid? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE id = ?", id);

			if (user == null)
			{
				return NotFound();
			}
			else
			{
				User = user;
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
			var user = await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE id = ?", id);

			if (user != null)
			{
				await _mapper.DeleteAsync<User>("WHERE id = ? IF EXISTS", id);
            }
			stopwatch.Stop();
			_logger.LogInformation("User Delete Time: {0}", stopwatch.ElapsedMilliseconds);
			if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com") return RedirectToPage("./Index");
            else
            {
                try
                {
                    HttpContext.Session.Clear();
                    return RedirectToPage("../Index");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return Page();
                }
            }
        }
	}
}
