﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using System.ServiceModel.Security;
using UCosmic.Domain.People;

namespace UCosmic.Domain.InstitutionalAgreements
{
    public class MyInstitutionalAgreementSettings :
        BaseEntityQuery<InstitutionalAgreementConfiguration>,
        IDefineQuery<InstitutionalAgreementConfiguration>
    {
        public MyInstitutionalAgreementSettings(IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
        internal bool IsWritable { get; set; }
    }

    public class HandleMyInstitutionalAgreementSettingsQuery :
        IHandleQueries<MyInstitutionalAgreementSettings,
        InstitutionalAgreementConfiguration>
    {
        private readonly IProcessQueries _queryProcessor;
        private readonly ICommandEntities _entities;

        public HandleMyInstitutionalAgreementSettingsQuery(IProcessQueries queryProcessor, ICommandEntities entities)
        {
            _queryProcessor = queryProcessor;
            _entities = entities;
        }

        public InstitutionalAgreementConfiguration Handle(MyInstitutionalAgreementSettings query)
        {
            if (query == null) throw new ArgumentNullException("query");

            // make sure user is authorized to view settings
            if (!query.Principal.IsInAnyRoles(RoleName.InstitutionalAgreementManagers))
                throw new SecurityAccessDeniedException(string.Format(
                    "User '{0}' does not have privileges to invoke this function.",
                        query.Principal.Identity.Name));

            // first, get the principal's affiliation
            var person = _queryProcessor.Execute(new MyPerson(query.Principal)
            {
                EagerLoad = new Expression<Func<Person, object>>[]
                {
                    p => p.Affiliations.Select(a => a.Establishment),
                },
            });

            // from person's default affiliation, determine establishment
            var configurations = (query.IsWritable)
                ? _entities.Get<InstitutionalAgreementConfiguration>()
                : _entities.Query<InstitutionalAgreementConfiguration>();
            configurations = configurations.EagerLoad(_entities, query.EagerLoad)
                .Where(c =>
                    c.ForEstablishment != null &&
                    c.ForEstablishment.IsMember &&
                    (
                        c.ForEstablishmentId == person.DefaultAffiliation.EstablishmentId ||
                        c.ForEstablishment.Offspring.Select(a => a.OffspringId)
                            .Contains(person.DefaultAffiliation.EstablishmentId)
                    )
                )
                .OrderBy(new Dictionary<Expression<Func<InstitutionalAgreementConfiguration, object>>, OrderByDirection>
                {
                    { c => c.ForEstablishment.Ancestors.Count, OrderByDirection.Descending}
                })
            ;

            return configurations.FirstOrDefault();
        }
    }
}
