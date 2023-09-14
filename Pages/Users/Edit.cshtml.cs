using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
    public class EditModel : PageModel
	{
		private readonly ILogger<EditModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public EditModel(SneakerShopContext context, ILogger<EditModel> logger)
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
			User = user;
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			User.Password = PasswordHasher.Hash(User.Password);

			try
			{
				await _mapper.UpdateAsync<User>("SET firstname = ?, lastname = ?, email = ?, password = ?, address = ?, phone = ? " +
					"WHERE id = ?", User.FirstName, User.LastName, User.Email, User.Password, User.Address, User.Phone, User.ID);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!await UserExistsAsync(User.ID))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			stopwatch.Stop();
			_logger.LogInformation("User Edit Time: {0}", stopwatch.ElapsedMilliseconds);
			if (HttpContext.Session.GetString("Email") == "admin@sneakershop.com") return RedirectToPage("./Index");
			else return Page();
		}

		private async Task<bool> UserExistsAsync(Guid id)
		{
			return await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE id = ?", id) != null;
		}
	}
}
