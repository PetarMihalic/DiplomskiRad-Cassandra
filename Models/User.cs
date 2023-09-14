using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.user")]
	public class User
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = string.Empty;
		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = string.Empty;
		[Required]
		public string Email { get; set; } = string.Empty;
		[Required]
		public string Password { get; set; } = string.Empty;
		[Required]
		public string Address { get; set; } = string.Empty;
		[Required]
		public string Phone { get; set; } = string.Empty;
		[Display(Name = "Created At")]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
	}
}
