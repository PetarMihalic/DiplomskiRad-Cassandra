using Cassandra.Mapping;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Users
{
	public class LoginModel : PageModel
	{
		public string errorMessage = "";
		private readonly ILogger<LoginModel> _logger;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;
		public LoginModel(SneakerShopContext context, ILogger<LoginModel> logger)
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

			var user = await _mapper.FirstOrDefaultAsync<User>("SELECT * FROM user WHERE Email = ? ALLOW FILTERING", User.Email);
			if (user == null)
			{
				return NotFound();
			}
			if (!PasswordHasher.Verify(User.Password, user.Password))
			{
				errorMessage = "Wrong password";
				return Page();
			}
			else
			{
				if (string.IsNullOrEmpty(HttpContext.Session.GetString("Name")))
				{
					HttpContext.Session.SetString("UserID", user.ID.ToString());
					HttpContext.Session.SetString("Name", user.FirstName + " " + user.LastName);
					HttpContext.Session.SetString("Email", user.Email);
				}
				//var cart = await _context.Carts.Where(m => m.UserID == user.ID).Include(c => c.Inventory)
				//.Include(c => c.user).ToListAsync();
				var cart = await _mapper.FetchAsync<Cart>("SELECT * FROM cart WHERE userID = ? ALLOW FILTERING", user.ID);

				HttpContext.Session.Remove("Cart");
				int cartQuantity = 0;
				foreach (var cartItem in cart)
				{
					cartQuantity += cartItem.Quantity;
				}
				HttpContext.Session.SetString("Cart", cartQuantity.ToString());

				stopwatch.Stop();
				_logger.LogInformation("User Login Time: {0}", stopwatch.ElapsedMilliseconds);

				return RedirectToPage("/Index");
			}
		}
	}
}
