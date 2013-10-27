﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using AttributeRouting;
using AttributeRouting.Web.Http;
using UCosmic.Domain.Activities;
using UCosmic.Domain.Employees;
using UCosmic.Domain.Establishments;
using UCosmic.Domain.Places;
using UCosmic.Web.Mvc.Models;
using System.IO;
using ImageResizer;

namespace UCosmic.Web.Mvc.ApiControllers
{
    [RoutePrefix("api")]
    public class EmployeesController : ApiController
    {
        private readonly IProcessQueries _queryProcessor;
        private readonly IQueryEntities _queryEntities;
        private readonly IStoreBinaryData _binaryData;

        public EmployeesController(IProcessQueries queryProcessor, IQueryEntities queryEntities, IStoreBinaryData binaryData)
        {
            _queryProcessor = queryProcessor;
            _queryEntities = queryEntities;
            _binaryData = binaryData;
        }

        [CacheHttpGet(Duration = 3600)]
        [GET("{domain}/employees/activities/places")]
        public IEnumerable<ActivitiesPlaceApiModel> GetActivitiesPlaces(string domain, [FromUri] ActivitiesPlacesInputModel input)
        {
            //throw new Exception();
            //System.Threading.Thread.Sleep(10000);

            // get the tenant id
            var tenant = _queryProcessor.Execute(new EstablishmentByDomain(domain));
            if (tenant == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            var tenantId = tenant.RevisionId;

            var publishedText = ActivityMode.Public.AsSentenceFragment();
            var activities = _queryEntities.Query<Activity>()
                .Where(x => x.Person.Affiliations.Any(y => y.IsDefault) // make sure person's default affiliation is not null
                    &&
                    (   // person's default affiliation is with or underneath the tenant domain being queried
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).EstablishmentId == tenantId
                        ||
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).Establishment.Ancestors.Any(y => y.AncestorId == tenantId)
                    )
                )
                .Where(x => x.Original == null && x.ModeText == publishedText) // published, non-work-copy
                .Select(x => x.Values.FirstOrDefault(y => y.ModeText == publishedText))
            ;

            var places = _queryEntities.Query<Place>().Where(x => x == null); // hack an empty places queryable
            if (input.Countries.HasValue && input.Countries.Value)
            {
                var countries = activities.SelectMany(x => x.Locations.Select(y => y.Place)) // get all places from locations collection
                    .Select(x => x.IsCountry // if the location is a country, select it
                        ? x
                        : x.Ancestors.Any(y => y.Ancestor.IsCountry) // otherwise select the location's parent country when one exists
                            ? x.Ancestors.FirstOrDefault(y => y.Ancestor.IsCountry).Ancestor
                            : null
                    )
                    .Where(x => x != null).Distinct()
                ;
                places = places.Union(countries);
            }
            if (input.PlaceIds != null && input.PlaceIds.Any())
            {
                var byId = activities.SelectMany(x => x.Locations.Select(y => y.Place)) // get all places from locations collection
                    .Select(x => input.PlaceIds.Contains(x.RevisionId) // if the location is the place, select it
                        ? x
                        : x.Ancestors.Any(y => input.PlaceIds.Contains(y.AncestorId)) // otherwise select the location's parent when one matches
                            ? x.Ancestors.FirstOrDefault(y => input.PlaceIds.Contains(y.AncestorId)).Ancestor
                            : null
                    )
                    .Where(x => x != null).Distinct()
                ;
                places = places.Union(byId);
            }

            var models = places.Select(place => new ActivitiesPlaceApiModel
            {
                PlaceId = place.RevisionId,
                PlaceName = place.OfficialName,
                IsCountry = place.IsCountry,
                CountryCode = place.IsCountry && place.GeoPlanetPlace != null
                    ? place.GeoPlanetPlace.Country.Code
                    : place.Ancestors.Any(node => node.Ancestor.IsCountry && node.Ancestor.GeoPlanetPlace != null)
                        ? place.Ancestors.FirstOrDefault(node => node.Ancestor.IsCountry && node.Ancestor.GeoPlanetPlace != null)
                            .Ancestor.GeoPlanetPlace.Country.Code
                        : null,
                ActivityIds = activities
                    .Where(activity =>
                        activity.Locations.Any(location => location.PlaceId == place.RevisionId) ||
                        activity.Locations.Any(location => location.Place.Ancestors.Any(node => node.AncestorId == place.RevisionId)))
                    .Select(activity => activity.ActivityId).Distinct(),
            })
            .ToArray();

            return models;
        }

        [CacheHttpGet(Duration = 3600)]
        [GET("{domain}/employees/people/places")]
        public IEnumerable<PeoplePlaceApiModel> GetPeoplePlaces(string domain, [FromUri] PeoplePlacesInputModel input)
        {
            //throw new Exception();
            //System.Threading.Thread.Sleep(10000);

            // get the tenant id
            var tenant = _queryProcessor.Execute(new EstablishmentByDomain(domain));
            if (tenant == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            var tenantId = tenant.RevisionId;

            var publishedText = ActivityMode.Public.AsSentenceFragment();
            var activities = _queryEntities.Query<Activity>()
                .Where(x => x.Person.Affiliations.Any(y => y.IsDefault) // make sure person's default affiliation is not null
                    &&
                    (   // person's default affiliation is with or underneath the tenant domain being queried
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).EstablishmentId == tenantId
                        ||
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).Establishment.Ancestors.Any(y => y.AncestorId == tenantId)
                    )
                )
                .Where(x => x.Original == null && x.ModeText == publishedText) // published, non-work-copy
                .Select(x => x.Values.FirstOrDefault(y => y.ModeText == publishedText))
            ;

            var places = _queryEntities.Query<Place>().Where(x => x == null); // hack an empty places queryable
            if (input.Countries.HasValue && input.Countries.Value)
            {
                var countries = activities.SelectMany(x => x.Locations.Select(y => y.Place)) // get all places from locations collection
                    .Select(x => x.IsCountry // if the location is a country, select it
                        ? x
                        : x.Ancestors.Any(y => y.Ancestor.IsCountry) // otherwise select the location's parent country when one exists
                            ? x.Ancestors.FirstOrDefault(y => y.Ancestor.IsCountry).Ancestor
                            : null
                    )
                    .Where(x => x != null).Distinct()
                ;
                places = places.Union(countries);
            }
            if (input.PlaceIds != null && input.PlaceIds.Any())
            {
                var byId = activities.SelectMany(x => x.Locations.Select(y => y.Place)) // get all places from locations collection
                    .Select(x => input.PlaceIds.Contains(x.RevisionId) // if the location is the place, select it
                        ? x
                        : x.Ancestors.Any(y => input.PlaceIds.Contains(y.AncestorId)) // otherwise select the location's parent when one matches
                            ? x.Ancestors.FirstOrDefault(y => input.PlaceIds.Contains(y.AncestorId)).Ancestor
                            : null
                    )
                    .Where(x => x != null).Distinct()
                ;
                places = places.Union(byId);
            }

            var models = places.Select(place => new PeoplePlaceApiModel
            {
                PlaceId = place.RevisionId,
                PlaceName = place.OfficialName,
                IsCountry = place.IsCountry,
                CountryCode = place.IsCountry && place.GeoPlanetPlace != null
                    ? place.GeoPlanetPlace.Country.Code
                    : place.Ancestors.Any(node => node.Ancestor.IsCountry && node.Ancestor.GeoPlanetPlace != null)
                        ? place.Ancestors.FirstOrDefault(node => node.Ancestor.IsCountry && node.Ancestor.GeoPlanetPlace != null)
                            .Ancestor.GeoPlanetPlace.Country.Code
                        : null,
                PersonIds = activities
                    .Where(activity =>
                        activity.Locations.Any(location => location.PlaceId == place.RevisionId) ||
                        activity.Locations.Any(location => location.Place.Ancestors.Any(node => node.AncestorId == place.RevisionId)))
                    .Select(activity => activity.Activity.Person.RevisionId).Distinct(),
            })
            .ToArray();

            return models;
        }

        [CacheHttpGet(Duration = 3600)]
        [GET("{domain}/employees/activities/summary")]
        public ActivitiesSummary GetActivitiesSummary(string domain)
        {
            //throw new Exception();
            //System.Threading.Thread.Sleep(2000);

            // get the tenant id
            var tenant = _queryProcessor.Execute(new EstablishmentByDomain(domain));
            if (tenant == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            var tenantId = tenant.RevisionId;

            var publishedText = ActivityMode.Public.AsSentenceFragment();
            var activities = _queryEntities.Query<Activity>()
                .Where(x => x.Person.Affiliations.Any(y => y.IsDefault) // make sure person's default affiliation is not null
                    &&
                    (   // person's default affiliation is with or underneath the tenant domain being queried
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).EstablishmentId == tenantId
                        ||
                        x.Person.Affiliations.FirstOrDefault(y => y.IsDefault).Establishment.Ancestors.Any(y => y.AncestorId == tenantId)
                    )
                )
                .Where(x => x.Original == null && x.ModeText == publishedText) // published, non-work-copy
                .Select(x => x.Values.FirstOrDefault(y => y.ModeText == publishedText))
            ;

            var model = new ActivitiesSummary
            {
                ActivityCount = activities.Count(),
                PersonCount = activities.Select(x => x.Activity.PersonId).Distinct().Count(),
                LocationCount = activities.SelectMany(x => x.Locations).Select(x => x.PlaceId).Distinct().Count(),
            };

            return model;
        }

        [CacheHttpGet(Duration = 3600)]
        [GET("{domain}/employees/settings/icons/{name}")]
        public HttpResponseMessage GetSettingsIcon(string domain, string name)
        {
            //throw new Exception();
            //System.Threading.Thread.Sleep(2000);

            if (!"global-view".Equals(name, StringComparison.OrdinalIgnoreCase) &&
                !"find-expert".Equals(name, StringComparison.OrdinalIgnoreCase))

                throw new HttpResponseException(HttpStatusCode.BadRequest);

            var pathAndMime = new { Path = "", Mime = "" };

            var tenant = _queryProcessor.Execute(new EstablishmentByDomain(domain));
            if (tenant != null)
            {
                var tenantId = tenant.RevisionId;
                var settings = _queryProcessor.Execute(new EmployeeModuleSettingsByEstablishmentId(tenantId));
                if (settings != null)
                {
                    if ("global-view".Equals(name, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(settings.GlobalViewIconName))
                    {
                        pathAndMime = new
                        {
                            Path = string.Format("{0}{1}", settings.GlobalViewIconPath, settings.GlobalViewIconFileName),
                            Mime = settings.GlobalViewIconMimeType,
                        };
                    }
                    else if ("find-expert".Equals(name, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(settings.FindAnExpertIconFileName))
                    {
                        pathAndMime = new
                        {
                            Path = string.Format("{0}{1}", settings.FindAnExpertIconPath, settings.FindAnExpertIconFileName),
                            Mime = settings.FindAnExpertIconMimeType,
                        };
                    }
                }
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            if (!string.IsNullOrWhiteSpace(pathAndMime.Path) && !string.IsNullOrWhiteSpace(pathAndMime.Mime))
            {
                var content = _binaryData.Get(pathAndMime.Path);
                if (content == null)
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                response.Content = new ByteArrayContent(content);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue(pathAndMime.Mime);
                return response;
            }
            var stream = new MemoryStream(); // do not dispose, StreamContent will dispose internally
            var relativePath = string.Format("~/{0}", Links.images.icons.globe.shiny_24_png);
            ImageBuilder.Current.Build(relativePath, stream, new ResizeSettings());
            stream.Position = 0;
            response.Content = new StreamContent(stream);
            var defaultMime = new MediaTypeHeaderValue("image/png");
            response.Content.Headers.ContentType = defaultMime;
            return response;
        }
    }
}