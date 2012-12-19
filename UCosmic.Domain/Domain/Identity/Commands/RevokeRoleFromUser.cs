﻿using System;
using System.Linq;

namespace UCosmic.Domain.Identity
{
    public class RevokeRoleFromUser
    {
        public RevokeRoleFromUser(Guid roleGuid, Guid userGuid)
        {
            if (roleGuid == Guid.Empty) throw new ArgumentException("Cannot be empty", "roleGuid");
            if (userGuid == Guid.Empty) throw new ArgumentException("Cannot be empty", "userGuid");
            RoleGuid = roleGuid;
            UserGuid = userGuid;
        }

        public Guid RoleGuid { get; private set; }
        public Guid UserGuid { get; private set; }
        public bool IsNewlyRevoked { get; internal set; }
    }

    public class HandleRevokeRoleFromUserCommand : IHandleCommands<RevokeRoleFromUser>
    {
        private readonly ICommandEntities _entities;

        public HandleRevokeRoleFromUserCommand(ICommandEntities entities)
        {
            _entities = entities;
        }

        public void Handle(RevokeRoleFromUser command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var grant = _entities.Get<RoleGrant>().SingleOrDefault(g =>
                g.Role.EntityId == command.RoleGuid &&
                g.User.EntityId == command.UserGuid);

            if (grant == null) return;
            _entities.Purge(grant);
            command.IsNewlyRevoked = true;
        }
    }
}
