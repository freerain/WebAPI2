using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Result
    {
        public string message_id { get; set; }
    }

    public class FirebaseJson
    {
        public long multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public IList<Result> results { get; set; }
    }
}