using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ShowTracker.MyAnimeListService.Models
{
    public class PagedResponseModel<T>
    {
        [JsonPropertyName("data")]
        public List<PagedResponseNodeModel<T>> Data { get; set; }
    }
}
