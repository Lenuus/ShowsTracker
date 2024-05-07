using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowsTracker.Application.Service.Email
{
    public class MailSettingsOptions
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }


        public string DisplayName { get; set; }
    }
}
