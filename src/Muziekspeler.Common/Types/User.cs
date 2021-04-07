using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Types
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = "Defaulter";

        public string Status { get; set; } = "Jammin'";
    }
}
