using Cassandra.Mapping.Attributes;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.cart")]
	public class Cart
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		public Guid InventoryID { get; set; }
		public Guid? UserID { get; set; }
		public string? SessionID { get; set; } = string.Empty;
		public int Quantity { get; set; }
		public Inventory Inventory { get; set; }
		public User user { get; set; }
	}
}
