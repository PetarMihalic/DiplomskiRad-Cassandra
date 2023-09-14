using Cassandra.Mapping.Attributes;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.inventory")]
	public class Inventory
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		public Guid SneakerID { get; set; }
		public float Size { get; set; }
		public int Quantity { get; set; }
		public Sneaker Sneaker { get; set; }
	}
}
