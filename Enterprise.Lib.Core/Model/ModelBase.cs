using System;
using Enterprise.Lib.Core.Interface;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Enterprise.Lib.Core.Model
{
    public abstract class ModelBase : IModel
    {
        public dynamic Id { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DataSrcId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int JobAuditId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime UpdatedOn { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        [NotMapped]
        public string Deleted { get; set; }
    }
}