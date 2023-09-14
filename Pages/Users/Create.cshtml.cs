using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
    public class CreateModel : PageModel
	{
		private readonly ILogger<CreateModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public CreateModel(SneakerShopContext context, ILogger<CreateModel> logger)
		{
			_logger = logger;
			_session = context.GetSession();
			_mapper = context.GetMapper();
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public User User { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
			if (User == null)
			{
				return Page();
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			User.Password = PasswordHasher.Hash(User.Password);
			await _mapper.InsertAsync(User);
			stopwatch.Stop();
			_logger.LogInformation("User Create Time: {0}", stopwatch.ElapsedMilliseconds);
            if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com")
            {
                return RedirectToPage("./Index");
            }
            else
            {
                return RedirectToPage("./Login");
            }
        }
	}
}
