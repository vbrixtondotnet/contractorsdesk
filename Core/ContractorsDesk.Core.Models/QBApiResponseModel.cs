using Newtonsoft.Json;

namespace ContractorsDesk.Core.Models
{
	public class QBApiResponseModel<T>
	{
		[JsonProperty("QueryResponse")]
		public QBQueryResponseModel<T> QBQueryResponse { get; set; }
	}
}
