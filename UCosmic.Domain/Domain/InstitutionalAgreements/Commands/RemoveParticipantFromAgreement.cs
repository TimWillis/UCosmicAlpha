﻿using System;
using System.Linq;
using System.Security.Principal;

namespace UCosmic.Domain.InstitutionalAgreements
{
    public class RemoveParticipantFromAgreement
    {
        public RemoveParticipantFromAgreement(IPrincipal principal, Guid establishmentGuid, Guid agreementGuid)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;

            if (establishmentGuid == Guid.Empty) throw new ArgumentException("Cannot be empty", "establishmentGuid");
            EstablishmentGuid = establishmentGuid;

            if (agreementGuid == Guid.Empty) throw new ArgumentException("Cannot be empty", "agreementGuid");
            AgreementGuid = agreementGuid;
        }

        public IPrincipal Principal { get; private set; }
        public Guid EstablishmentGuid { get; private set; }
        public Guid AgreementGuid { get; private set; }
        public bool IsNewlyRemoved { get; internal set; }
    }

    public class HandleRemoveParticipantFromAgreementCommand : IHandleCommands<RemoveParticipantFromAgreement>
    {
        private readonly ICommandEntities _entities;

        public HandleRemoveParticipantFromAgreementCommand(ICommandEntities entities)
        {
            _entities = entities;
        }

        public void Handle(RemoveParticipantFromAgreement command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // todo: this should be FindByPrimaryKey
            var entity = _entities.Get<InstitutionalAgreementParticipant>()
                .SingleOrDefault(x =>
                    x.Establishment.EntityId == command.EstablishmentGuid &&
                    x.Agreement.EntityId == command.AgreementGuid
                );

            if (entity == null) return;
            _entities.Purge(entity);
            command.IsNewlyRemoved = true;
        }
    }
}
