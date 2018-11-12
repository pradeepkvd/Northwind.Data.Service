using Newtonsoft.Json;

namespace Enterprise.Lib.Core.Interface
{
    public interface IModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        dynamic Id { get; set; }
    }
}