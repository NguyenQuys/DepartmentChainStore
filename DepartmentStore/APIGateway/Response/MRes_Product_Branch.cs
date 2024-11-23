using System.Text.Json.Serialization;

namespace APIGateway.Response
{
	public class MRes_Product_Branch
	{
		[JsonPropertyName("id")]
		public int Id { get; set; }

		[JsonPropertyName("productName")]
		public string ProductName { get; set; }

		[JsonPropertyName("idProductCategory")]
		public int IdProductCategory { get; set; }

		[JsonPropertyName("price")]
		public decimal Price { get; set; }

		[JsonPropertyName("isHide")]
		public bool IsHide { get; set; }

		[JsonPropertyName("batchNumber")]
		public string BatchNumber { get; set; }

		[JsonPropertyName("quantity")]
		public int Quantity { get; set; }

		[JsonPropertyName("mainImage")]
		public string MainImage { get; set; }
	}
}
