using Cassandra.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SneakerShopCassandra.Data;
using SneakerShopCassandra.Models;
using System.Diagnostics;

namespace SneakerShopCassandra.Pages.Sneakers
{
    public class EditModel : PageModel
	{
		public IFormFile? imageFile1;
		public IFormFile? imageFile2;
		private readonly ILogger<EditModel> _logger;
		private readonly Mapper _mapper;
		public EditModel(SneakerShopContext context, ILogger<EditModel> logger)
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

			var sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?", id);
			if (sneaker == null)
			{
				return NotFound();
			}
			Sneaker = sneaker;
			return Page();
		}
		public async Task<IActionResult> OnPostAsync()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			var sneaker = await _mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?", Sneaker.ID);

			if (Request.Form.Files.GetFile("picture1") == null)
			{
				Sneaker.Picture1 = sneaker.Picture1;
			}
			else
			{
				imageFile1 = Request.Form.Files.GetFile("picture1");
				MemoryStream dataStream = new MemoryStream();
				await imageFile1.CopyToAsync(dataStream);
				Sneaker.Picture1 = dataStream.ToArray();
			}
			if (Request.Form.Files.GetFile("picture2") == null)
			{
				Sneaker.Picture1 = sneaker.Picture1;
			}
			else
			{
				imageFile2 = Request.Form.Files.GetFile("picture2");
				MemoryStream dataStream = new MemoryStream();
				await imageFile2.CopyToAsync(dataStream);
				Sneaker.Picture2 = dataStream.ToArray();
			}

			try
			{
                await _mapper.UpdateAsync<Sneaker>("SET brand = ?, name = ?, " +
					"description = ?, picture1 = ?, picture2 = ?, price = ?" +
					"  WHERE id = ? IF EXISTS", 
					Sneaker.Brand, Sneaker.Name, Sneaker.Description, 
					Sneaker.Picture1, Sneaker.Picture2, Sneaker.Price, Sneaker.ID);
            }
			catch (DbUpdateConcurrencyException)
			{
				if (!SneakerExists(Sneaker.ID))
				{
					return NotFound();
				}
				else
				{
					throw;
				}
			}
			stopwatch.Stop();
			_logger.LogInformation("Sneaker Edit Time: {0}", stopwatch.ElapsedMilliseconds);
			return RedirectToPage("/Index");
		}

		private bool SneakerExists(Guid id)
		{
			return !_mapper.FirstOrDefaultAsync<Sneaker>("SELECT * FROM sneaker WHERE id = ?",id).Equals(null);
		}
	}
}
