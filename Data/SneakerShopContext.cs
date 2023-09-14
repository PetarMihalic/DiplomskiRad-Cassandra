using Cassandra;
using Cassandra.Mapping;
using SneakerShopCassandra.Models;
using System.Net;

namespace SneakerShopCassandra.Data
{
	public class SneakerShopContext
	{
		private readonly Cluster _cluster;
		private readonly Cassandra.ISession _session;
		private readonly Mapper _mapper;

		public SneakerShopContext(IConfiguration configuration)
		{
			var contactPoints = configuration["Cassandra:ContactPoints"];
			var keyspace = configuration["Cassandra:Keyspace"];

			_cluster = Cluster.Builder()
				.AddContactPoints(contactPoints.Split(','))
				.Build();

			_session = _cluster.Connect(keyspace);
			_mapper = new Mapper(_session);

			_session.UserDefinedTypes.Define(
				UdtMap.For<Inventory>(),
				UdtMap.For<Sneaker>(),
				UdtMap.For<Orders>(),
				UdtMap.For<User>(),
				UdtMap.For<OrderDetails>()
			);
		}

		public Cassandra.ISession GetSession()
		{
			return _session;
		}

		public Mapper GetMapper()
		{
			return _mapper;
		}

		public async Task<List<Sneaker>> GetAllSneakers()
		{
			var sneakers= await _mapper.FetchAsync<Sneaker>();
			return sneakers.ToList();
		}

		public async Task<IList<Inventory>> GetInventory()
		{
			var iventory = await _mapper.FetchAsync<Inventory>();
			return iventory.ToList();
		}

		public async Task<IList<Orders>> GetAllOrders()
		{
			var orders = await _mapper.FetchAsync<Orders>();
			return orders.ToList();
		}

		public async Task<IList<User>> GetAllUsers()
		{
			var users = await _mapper.FetchAsync<User>();
			return users.ToList();
		}

	}
}
