﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using UCosmic.Domain.Languages;

namespace UCosmic.Domain.Places
{
    internal static class GeoNamesConverter
    {
        internal static GeoNamesToponym ToEntity(this NGeo.GeoNames.Toponym geoNamesToponym)
        {
            return (geoNamesToponym != null) ? Mapper.Map<GeoNamesToponym>(geoNamesToponym) : null;
        }

        internal static GeoNamesCountry ToEntity(this NGeo.GeoNames.Country geoNamesCountry)
        {
            return (geoNamesCountry != null) ? Mapper.Map<GeoNamesCountry>(geoNamesCountry) : null;
        }

        internal static PlaceName ToEntity(this GeoNamesAlternateName geoNamesAlternateName, ICommandEntities entities)
        {
            if (geoNamesAlternateName == null) return null;

            var placeName = Mapper.Map<PlaceName>(geoNamesAlternateName);

            if (!string.IsNullOrWhiteSpace(placeName.TranslationToHint))
            {
                placeName.TranslationToLanguage = entities.Get<Language>().ByIsoCode(placeName.TranslationToHint);
            }

            return placeName;
        }

        internal static ICollection<PlaceName> ToEntities(this IEnumerable<GeoNamesAlternateName> geoNamesAlternateNames, ICommandEntities entities)
        {
            if (geoNamesAlternateNames == null) return null;

            var placeNames = new List<PlaceName>();
            geoNamesAlternateNames.ToList().ForEach(n => placeNames.Add(n.ToEntity(entities)));

            return placeNames;
        }

        internal static Place ToPlace(this GeoNamesToponym toponym)
        {
            if (toponym == null) return null;

            var place = Mapper.Map<Place>(toponym);
            place.GeoNamesToponym = toponym;
            return place;
        }
    }

    public static class GeoNamesProfiler
    {
        public class ToponymToEntityProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<NGeo.GeoNames.Toponym, GeoNamesToponym>()
                    .ForMember(target => target.Center, opt => opt.ResolveUsing(source => new Coordinates(source.Latitude, source.Longitude)))
                    .ForMember(target => target.FeatureCode, opt => opt.ResolveUsing(source =>
                        source.FeatureCode))
                    .ForMember(target => target.Feature, opt => opt.ResolveUsing(source => new GeoNamesFeature
                    {
                        Code = source.FeatureCode,
                        Name = source.FeatureName,
                        ClassCode = source.FeatureClassCode,
                        Class = new GeoNamesFeatureClass { Code = source.FeatureClassCode, Name = source.FeatureClassName }
                    }))
                    .ForMember(target => target.Parent, opt => opt.Ignore())
                    .ForMember(target => target.AsCountry, opt => opt.Ignore())
                    .ForMember(target => target.Place, opt => opt.Ignore())
                    .ForMember(target => target.Children, opt => opt.Ignore())
                    .ForMember(target => target.Ancestors, opt => opt.Ignore())
                    .ForMember(target => target.Offspring, opt => opt.Ignore())
                ;
            }
        }

        public class AlternateNameToEntityProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<NGeo.GeoNames.AlternateName, GeoNamesAlternateName>()
                    .ForMember(target => target.AlternateNameId, opt => opt.Ignore())
                    .ForMember(target => target.GeoNameId, opt => opt.Ignore())
                    .ForMember(target => target.Toponym, opt => opt.Ignore())
                ;
            }
        }

        public class TimeZoneToEntityProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<NGeo.GeoNames.TimeZone, GeoNamesTimeZone>();
            }
        }

        public class CountryToEntityProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<NGeo.GeoNames.Country, GeoNamesCountry>()
                    .ForMember(target => target.BoundingBox, opt => opt.ResolveUsing(source =>
                        new BoundingBox(source.BoundingBoxNorth, source.BoundingBoxEast, source.BoundingBoxSouth, source.BoundingBoxWest)))
                    .ForMember(target => target.Code, opt => opt.ResolveUsing(source => source.CountryCode))
                    .ForMember(target => target.Name, opt => opt.ResolveUsing(source => source.CountryName))
                    .ForMember(target => target.AsToponym, opt => opt.Ignore())
                ;
            }
        }

        public class GeoNamesToponymToPlaceProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<GeoNamesToponym, Place>()
                    .ForMember(target => target.OfficialName, opt => opt.ResolveUsing(source => source.Name))
                    .ForMember(target => target.Names, opt => opt.ResolveUsing(source => source.AlternateNames))
                    .ForMember(target => target.IsEarth, opt => opt.ResolveUsing(source =>
                        source.GeoNameId == GeoNamesToponym.EarthGeoNameId))
                    .ForMember(target => target.IsContinent, opt => opt.ResolveUsing(source =>
                        source.FeatureCode == GeoNamesFeatureEnum.Continent.GetCode()
                            && source.Feature.ClassCode == GeoNamesFeatureClassEnum.Area.GetCode()))
                    .ForMember(target => target.IsCountry, opt => opt.ResolveUsing(source =>
                        source.AsCountry != null))
                    //.ForMember(target => target.IsBodyOfWater, opt => opt.Ignore())
                    .ForMember(target => target.BoundingBox, opt => opt.ResolveUsing(source =>
                        (source.AsCountry != null) ? source.AsCountry.BoundingBox : new BoundingBox(null, null, null, null)))
                    .ForMember(target => target.IsAdmin1, opt => opt.ResolveUsing(source =>
                        source.FeatureCode == GeoNamesFeatureEnum.AdministrativeDivisionLevel1.GetCode()
                            && source.Feature.ClassCode == GeoNamesFeatureClassEnum.AdministrativeBoundary.GetCode()))
                    .ForMember(target => target.IsAdmin2, opt => opt.ResolveUsing(source =>
                        source.FeatureCode == GeoNamesFeatureEnum.AdministrativeDivisionLevel2.GetCode()
                            && source.Feature.ClassCode == GeoNamesFeatureClassEnum.AdministrativeBoundary.GetCode()))
                    .ForMember(target => target.IsAdmin3, opt => opt.ResolveUsing(source =>
                        source.FeatureCode == GeoNamesFeatureEnum.AdministrativeDivisionLevel3.GetCode()
                            && source.Feature.ClassCode == GeoNamesFeatureClassEnum.AdministrativeBoundary.GetCode()))
                    .ForMember(target => target.IsWater, opt => opt.MapFrom(s => s.Feature.ClassCode == "H"))
                    .ForMember(target => target.IsRegion, opt => opt.MapFrom(s => s.FeatureCode == "AREA" || s.FeatureCode == "RGN"))
                    .ForMember(target => target.ParentId, opt => opt.Ignore())
                    .ForMember(target => target.Parent, opt => opt.Ignore())
                    .ForMember(target => target.Children, opt => opt.Ignore())
                    .ForMember(target => target.Ancestors, opt => opt.Ignore())
                    .ForMember(target => target.Offspring, opt => opt.Ignore())
                    .ForMember(target => target.RevisionId, opt => opt.Ignore())
                    .ForMember(target => target.EntityId, opt => opt.Ignore())
                    .ForMember(target => target.CreatedOnUtc, opt => opt.Ignore())
                    .ForMember(target => target.CreatedByPrincipal, opt => opt.Ignore())
                    .ForMember(e => e.UpdatedOnUtc, opt => opt.Ignore())
                    .ForMember(e => e.UpdatedByPrincipal, opt => opt.Ignore())
                    .ForMember(target => target.Version, opt => opt.Ignore())
                    .ForMember(target => target.IsCurrent, opt => opt.Ignore())
                    .ForMember(target => target.IsArchived, opt => opt.Ignore())
                    .ForMember(target => target.IsDeleted, opt => opt.Ignore())
                    .ForMember(target => target.GeoPlanetPlace, opt => opt.Ignore())
                    .ForMember(target => target.GeoNamesToponym, opt => opt.Ignore()) // configure to map `this`?
                ;
            }
        }

        public class GeoNamesAlternateNameToPlaceNameProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<GeoNamesAlternateName, PlaceName>()
                    .ForMember(target => target.Text, opt => opt.ResolveUsing(source => source.Name))
                    .ForMember(target => target.TranslationToHint, opt => opt.ResolveUsing(source => source.Language))
                    .ForMember(target => target.NameFor, opt => opt.Ignore())
                    .ForMember(target => target.TranslationToLanguage, opt => opt.Ignore())
                    .ForMember(target => target.IsPreferredTranslation, opt => opt.Ignore())
                    .ForMember(target => target.AsciiEquivalent, opt => opt.Ignore())
                    .ForMember(target => target.RevisionId, opt => opt.Ignore())
                    .ForMember(target => target.EntityId, opt => opt.Ignore())
                    .ForMember(target => target.CreatedOnUtc, opt => opt.Ignore())
                    .ForMember(target => target.CreatedByPrincipal, opt => opt.Ignore())
                    .ForMember(e => e.UpdatedOnUtc, opt => opt.Ignore())
                    .ForMember(e => e.UpdatedByPrincipal, opt => opt.Ignore())
                    .ForMember(target => target.Version, opt => opt.Ignore())
                    .ForMember(target => target.IsCurrent, opt => opt.Ignore())
                    .ForMember(target => target.IsArchived, opt => opt.Ignore())
                    .ForMember(target => target.IsDeleted, opt => opt.Ignore())
                ;
            }
        }
    }
}