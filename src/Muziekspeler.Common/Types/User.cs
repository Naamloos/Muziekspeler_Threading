using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Types
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = "User" + new Random().Next();

        public string Status { get; set; } = "Jammin'";

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
