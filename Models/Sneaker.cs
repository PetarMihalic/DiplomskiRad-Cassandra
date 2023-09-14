using Cassandra.Mapping.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SneakerShopCassandra.Models
{
	[Table("sneakershop.sneaker")]
	public class Sneaker
	{
		public Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		public string Brand { get; set; } = string.Empty;
		[Required]
		public string Name { get; set; } = string.Empty;
		[Required]
		public string Description { get; set; } = string.Empty;
		public byte[]? Picture1 { get; set; }
		public byte[]? Picture2 { get; set; }
		[Required]
		[DataType(DataType.Currency)]
		public decimal Price { get; set; }
	}
}
