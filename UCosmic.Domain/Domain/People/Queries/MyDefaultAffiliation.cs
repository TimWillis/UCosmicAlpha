﻿using System;
using System.Linq;
using System.Security.Principal;

namespace UCosmic.Domain.People
{
    public class MyDefaultAffiliation : BaseEntityQuery<Affiliation>, IDefineQuery<Affiliation>
    {
        public MyDefaultAffiliation(IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
    }

    public class HandleMyDefaultAffiliationQuery : IHandleQueries<MyDefaultAffiliation, Affiliation>
    {
        private readonly IQueryEntities _entities;

        public HandleMyDefaultAffiliationQuery(IQueryEntities entities)
        {
            _entities = entities;
        }

        public Affiliation Handle(MyDefaultAffiliation query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var entity = _entities.Query<Affiliation>()
                .EagerLoad(_entities, query.EagerLoad)
                .SingleOrDefault(x => x.IsDefault && x.Person.User != null &&
                    x.Person.User.Name.Equals(query.Principal.Identity.Name, StringComparison.OrdinalIgnoreCase));

            return entity;
        }
    }
}
