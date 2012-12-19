﻿using System;

namespace UCosmic.Domain.Places
{
    public class PlaceByGeoNameId : BaseEntityQuery<Place>, IDefineQuery<Place>
    {
        public PlaceByGeoNameId(int geoNameId)
        {
            GeoNameId = geoNameId;
        }

        public int GeoNameId { get; protected set; }
    }

    public class HandlePlaceByGeoNameIdQuery : IHandleQueries<PlaceByGeoNameId, Place>
    {
        private readonly IProcessQueries _queryProcessor;
        private readonly ICommandEntities _entities;
        private readonly IUnitOfWork _unitOfWork;

        public HandlePlaceByGeoNameIdQuery(IProcessQueries queryProcessor
            , ICommandEntities entities
            , IUnitOfWork unitOfWork
        )
        {
            _entities = entities;
            _queryProcessor = queryProcessor;
            _unitOfWork = unitOfWork;
        }

        public Place Handle(PlaceByGeoNameId query)
        {
            if (query == null) throw new ArgumentNullException("query");

            // first look in the db
            var place = _entities.Get<Place>()
                .EagerLoad(_entities, query.EagerLoad)
                .ByGeoNameId(query.GeoNameId)
            ;
            if (place != null) return place;

            // load toponym from storage
            var toponym = _queryProcessor.Execute(
                new SingleGeoNamesToponym(query.GeoNameId));

            // convert toponym to place
            place = toponym.ToPlace();

            // match place hierarchy to toponym hierarchy
            if (toponym.Parent != null)
            {
                place.Parent = Handle(
                    new PlaceByGeoNameId(toponym.Parent.GeoNameId));
            }

            // try to match to geoplanet
            if (place.GeoPlanetPlace == null && place.GeoNamesToponym != null)
            {
                place.GeoPlanetPlace = _queryProcessor.Execute(
                    new SingleGeoPlanetPlaceByGeoNameId(place.GeoNamesToponym.GeoNameId));
            }

            // add to db & save
            _entities.Create(place);
            _unitOfWork.SaveChanges();

            return place;
        }
    }
}
