﻿using AutoMapper;
using UCosmic.Domain.Degrees;
using UCosmic.Domain.Establishments;

namespace UCosmic.Web.Mvc.Models
{
    public class DegreePublicViewModel
    {
        public string Title { get; set; }
        public string FieldOfStudy { get; set; }
        public int? YearAwarded { get; set; }
        public Establishment Institution { get; set; }
        public int Id { get; set; }
    }

    public class PageOfDegreePublicViewModel : PageOf<DegreePublicViewModel>
    {
    }

    public static class DegreePublicViewProfiler
    {
        public class EntityToModel : Profile
        {
            protected override void Configure()
            {
                CreateMap<Degree, DegreePublicViewModel>()
                    .ForMember(d => d.Id, o => o.MapFrom(s => s.RevisionId))
                    ;
            }
        }

        public class PageQueryResultToPageOfItems : Profile
        {
            protected override void Configure()
            {
                CreateMap<PagedQueryResult<Degree>, PageOfDegreePublicViewModel>();
            }
        }
    }

}