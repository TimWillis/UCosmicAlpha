﻿using System;
using System.Linq;
using System.Security.Principal;

namespace UCosmic
{
    public static class RoleName
    {
        public const string SecurityAdministrator = "Security Administrator";
        public const string AuthorizationAgent = "Authorization Agent";
        public const string SecurityAdministrators = "Security Administrator,Authorization Agent";
        public const string AuthenticationAgent = "Authentication Agent";

        public const string InstitutionalAgreementSupervisor = "Institutional Agreement Supervisor";
        public const string InstitutionalAgreementManager = "Institutional Agreement Manager";
        public const string InstitutionalAgreementManagers = "Institutional Agreement Supervisor,Institutional Agreement Manager";

        public const string EmployeeProfileManager = "Employee Profile Manager";

        public const string EstablishmentLocationAgent = "Establishment Location Agent";

        public const string ElmahViewer = "Elmah Viewer";
        public const string EstablishmentAdministrator = "Establishment Administrator";

        public static bool IsInAnyRoles(this IPrincipal principal, string commaSeparatedRoles)
        {
            if (string.IsNullOrWhiteSpace(commaSeparatedRoles))
                throw new ArgumentException("Cannot be null or white space.", "commaSeparatedRoles");
            var splitRoles = commaSeparatedRoles.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return splitRoles.Any(principal.IsInRole);
        }
    }
}
