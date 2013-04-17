﻿using System;
using System.Collections.Generic;
using System.Linq;
using NGeo.Yahoo.GeoPlanet;

namespace UCosmic.Domain.Places
{
    public class SingleGeoPlanetPlace : IDefineQuery<GeoPlanetPlace>
    {
        public SingleGeoPlanetPlace(int woeId)
        {
            WoeId = woeId;
            Ignores = new List<GeoPlanetPlace>();
        }

        public int WoeId { get; private set; }
        internal List<GeoPlanetPlace> Ignores { get; set; }
    }

    public class HandleSingleGeoPlanetPlaceQuery : IHandleQueries<SingleGeoPlanetPlace, GeoPlanetPlace>
    {
        private readonly ICommandEntities _entities;
        private readonly IContainGeoPlanet _geoPlanet;

        public HandleSingleGeoPlanetPlaceQuery(ICommandEntities entities
            , IContainGeoPlanet geoPlanet
        )
        {
            _entities = entities;
            _geoPlanet = geoPlanet;
        }

        public GeoPlanetPlace Handle(SingleGeoPlanetPlace query)
        {
            if (query == null) throw new ArgumentNullException("query");

            // first look in the db
            var place = _entities.FindByPrimaryKey<GeoPlanetPlace>(query.WoeId);
            if (place != null) return place;

            // invoke geoplanet service
            var geoPlanetPlace = _geoPlanet.Place(query.WoeId);
            if (geoPlanetPlace == null) return null;

            // convert yahoo type to entity
            place = geoPlanetPlace.ToEntity();

            // map parent
            var ancestors = _geoPlanet.Ancestors(query.WoeId, RequestView.Long);
            if (ancestors != null && ancestors.Items.Count > 0)
            {
                var ancestor = ancestors.FirstOrDefault(x => !query.Ignores.Select(y => y.WoeId).Contains(x.WoeId));
                if (ancestor != null)
                    place.Parent = Handle(new SingleGeoPlanetPlace(ancestor.WoeId));
            }

            // add all belongtos
            place.BelongTos = place.BelongTos ?? new List<GeoPlanetPlaceBelongTo>();
            var geoPlanetBelongTos = _geoPlanet.BelongTos(query.WoeId);
            if (geoPlanetBelongTos != null && geoPlanetBelongTos.Items.Count > 0)
            {
                var rank = 0;
                foreach (var geoPlanetBelongTo in geoPlanetBelongTos.Items
                    .Where(x => !query.Ignores.Select(y => y.WoeId).Contains(x.WoeId)))
                {
                    place.BelongTos.Add(new GeoPlanetPlaceBelongTo
                    {
                        Rank = rank++,
                        BelongsTo = Handle(new SingleGeoPlanetPlace(geoPlanetBelongTo.WoeId)
                        {
                            Ignores = new List<GeoPlanetPlace>
                            {
                                place,
                            },
                        })
                    });
                }
            }

            // ensure no duplicate place types are added to db
            place.Type = new HandleSingleGeoPlanetPlaceTypeQuery(_entities, _geoPlanet)
                .Handle(new SingleGeoPlanetPlaceType(place.Type.Code));

            // map ancestors
            DeriveNodes(place);

            // add to db and save
            _entities.Create(place);

            return place;
        }

        private void DeriveNodes(GeoPlanetPlace place)
        {
            place.Ancestors.ToList().ForEach(node =>
                _entities.Purge(node));

            var separation = 1;
            var parent = place.Parent;
            while (parent != null)
            {
                place.Ancestors.Add(new GeoPlanetPlaceNode
                {
                    Ancestor = parent,
                    Separation = separation++,
                });
                parent = parent.Parent;
            }
        }
    }
}
