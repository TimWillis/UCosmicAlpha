﻿using System;
using System.Linq;

namespace UCosmic.Domain.Establishments
{
    public class EstablishmentNames : BaseEntitiesQuery<EstablishmentName>, IDefineQuery<EstablishmentName[]>
    {
        public EstablishmentNames(int? establishmentId = null)
        {
            EstablishmentId = establishmentId;
        }

        public int? EstablishmentId { get; private set; }
    }

    public class HandleEstablishmentNamesQuery : IHandleQueries<EstablishmentNames, EstablishmentName[]>
    {
        private readonly IQueryEntities _entities;

        public HandleEstablishmentNamesQuery(IQueryEntities entities)
        {
            _entities = entities;
        }

        public EstablishmentName[] Handle(EstablishmentNames query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var results = _entities.Query<EstablishmentName>()
                .EagerLoad(_entities, query.EagerLoad)
                .Where(x => query.EstablishmentId.HasValue
                    ? x.ForEstablishment.RevisionId == query.EstablishmentId.Value
                    : x.RevisionId != 0)
                .OrderBy(query.OrderBy)
            ;

            return results.ToArray();
        }
    }
}