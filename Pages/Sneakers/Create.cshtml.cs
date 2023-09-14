using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Sneakers
{
    public class CreateModel : PageModel
	{
		public IFormFile? imageFile1;
		public IFormFile? imageFile2;
		private readonly ILogger<CreateModel> _logger;
		private readonly Mapper _mapper;
		public CreateModel(SneakerShopContext context, ILogger<CreateModel> logger)
		{
			_logger = logger;
			_mapper = context.GetMapper();
		}

		public IActionResult OnGet()
		{
			return Page();
		}

		[BindProperty]
		public Sneaker Sneaker { get; set; } = default!;

		public async Task<IActionResult> OnPostAsync()
		{
			if (Sneaker == null)
			{
				return Page();
			}

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			imageFile1 = Request.Form.Files.GetFile("picture1");
			imageFile2 = Request.Form.Files.GetFile("picture2");

			MemoryStream dataStream = new MemoryStream();
			await imageFile1.CopyToAsync(dataStream);
			Sneaker.Picture1 = dataStream.ToArray();
			dataStream = new MemoryStream();
			await imageFile2.CopyToAsync(dataStream);
			Sneaker.Picture2 = dataStream.ToArray();

			await _mapper.InsertAsync(Sneaker);

			stopwatch.Stop();
			_logger.LogInformation("Sneaker Create Time: {0}", stopwatch.ElapsedMilliseconds);

			return RedirectToPage("/Index");
		}
	}
}
