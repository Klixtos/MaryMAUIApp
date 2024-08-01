using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MaryAgent.Service.Models
{
    public class MaryResponse
    {
        public string? response { get; set; }
        public float? cost { get; set; }
        public float? balance { get; set; }
    }
}
