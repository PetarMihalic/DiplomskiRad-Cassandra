using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.orders")]
	public class Orders
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		[DisplayFormat(NullDisplayText = "Guest")]
		public Guid? UserID { get; set; }
		public string Name { get; set; }
		[Display(Name = "Created At")]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		[Required]
		[Display(Name = "Payment Type")]
		public string PaymentType { get; set; } = string.Empty;
		public string Status { get; set; } = "pending";
		public List<OrderDetails> OrderDetails { get; set; }
	}
}
