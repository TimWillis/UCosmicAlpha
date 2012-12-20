﻿using System.Security.Principal;
using UCosmic.Domain.Identity;

namespace UCosmic.SeedData
{
    public class RoleEntitySeeder : BaseRoleSeeder, ISeedData
    {
        public RoleEntitySeeder(IProcessQueries queryProcessor
            , IHandleCommands<CreateRole> createRole
            , IHandleCommands<UpdateRole> updateRole
            , IUnitOfWork unitOfWork
        )
            : base(queryProcessor, createRole, updateRole, unitOfWork)
        {
        }

        public void Seed()
        {
            Seed(RoleName.AuthorizationAgent,
                "Authorization Agents can control the roles and role grants to any user, regardless of establishment."
            );
            Seed(RoleName.InstitutionalAgreementManager,
                "Institutional Agreement Managers can add, edit, and otherwise manage institutional agreements for their institutions. " +
                "Additionally, they are allowed to view fields marked with 'private' access."
            );
            Seed(RoleName.InstitutionalAgreementSupervisor,
                "Institutional Agreement Supervisors can grant Institutional Agreement Manager " +
                "privileges to users at their institutions."
            );
            Seed(RoleName.AuthenticationAgent,
                "Authentication Agents can sign on as any user, regardless of establishment."
            );
            Seed(RoleName.EstablishmentLocationAgent,
                "Establishment Location Agents can modify location information for any establishment."
            );
            Seed(RoleName.EstablishmentAdministrator,
                "Establishment Administrators can add, edit, and otherwise manage shared establishment data."
            );
            Seed(RoleName.ElmahViewer,
                "Elmah Viewers can view the ELMAH error logs."
            );
        }
    }

    public abstract class BaseRoleSeeder
    {
        private readonly IProcessQueries _queryProcessor;
        private readonly IHandleCommands<CreateRole> _createRole;
        private readonly IHandleCommands<UpdateRole> _updateRole;
        private readonly IUnitOfWork _unitOfWork;

        protected BaseRoleSeeder(IProcessQueries queryProcessor
            , IHandleCommands<CreateRole> createRole
            , IHandleCommands<UpdateRole> updateRole
            , IUnitOfWork unitOfWork
        )
        {
            _queryProcessor = queryProcessor;
            _createRole = createRole;
            _updateRole = updateRole;
            _unitOfWork = unitOfWork;
        }

        protected void Seed(string roleName, string roleDescription)
        {
            var role = _queryProcessor.Execute(new RoleBySlug(roleName.Replace(" ", "-")));
            if (role == null)
            {
                _createRole.Handle(new CreateRole(roleName) { Description = roleDescription, });
            }
            else
            {
                var identity = new GenericIdentity(GetType().Name);
                var principal = new GenericPrincipal(identity, new[] { RoleName.AuthorizationAgent });
                _updateRole.Handle(new UpdateRole(principal)
                {
                    EntityId = role.EntityId,
                    Description = roleDescription,
                });
            }
            _unitOfWork.SaveChanges();
        }
    }
}