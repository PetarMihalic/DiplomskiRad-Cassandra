using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.orderdetails")]
	public class OrderDetails
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		public Guid InventoryID { get; set; }
		public Guid OrderID { get; set; }
		public int Quantity { get; set; }
		public Inventory Inventory { get; set; }
		public Orders Orders { get; set; }
	}
}
