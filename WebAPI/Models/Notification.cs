using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class Notification
    {
        public String Token { get; set; }
        public String Title { get; set; }
        public String Body { get; set; }
        public String Icon { get; set; }
        public String Image { get; set; }
        public String Url { get; set; }

    }
}