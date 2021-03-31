using System;
using System.Collections.Generic;
using System.Text;

namespace Muziekspeler.Common.Types
{
    public class User
    {
        public int Id { get; set; }

        public string DisplayName { get; set; } = "Defaulter";

        public string Status = "Jammin'";

        public User() {
            // Id = userID komt uit server, onbekend hoe ATM;
        }

        public void setDisplayName(string name) {
            DisplayName = name;
        }

        public string getDisplayName() {
            return DisplayName;
        }

        public void setStatus(string status) {
            Status = status;
        }

        public string getStatus() {
            return Status;
        }
        // Client stuurt displayName en status naar server, server verstuurt user object terug
    }
}
