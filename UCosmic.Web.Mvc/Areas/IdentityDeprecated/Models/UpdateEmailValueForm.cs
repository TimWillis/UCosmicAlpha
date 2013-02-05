﻿using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AutoMapper;
using FluentValidation;
using UCosmic.Domain.People;
using UCosmic.Web.Mvc.Models;

namespace UCosmic.Web.Mvc.Areas.IdentityDeprecated.Models
{
    public class UpdateEmailValueForm : IReturnUrl
    {
        [DataType(DataType.EmailAddress)]
        [Display(Name = ValueDisplayName)]
        //[Remote("ValidateValue", "UpdateEmailValue", "Identity", HttpMethod = "POST", AdditionalFields = "PersonUserName,Number")]
        public string Value { get; set; }
        public const string ValuePropertyName = "Value";
        public const string ValueDisplayName = "New spelling";

        [Display(Name = OldSpellingDisplayName)]
        public string OldSpelling { get; set; }
        public const string OldSpellingDisplayName = "Current spelling";

        [HiddenInput(DisplayValue = false)]
        public string PersonUserName { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string ReturnUrl { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int Number { get; set; }
    }

    public class UpdateEmailValueValidator : AbstractValidator<UpdateEmailValueForm>
    {
        public const string FailedBecausePreviousSpellingDoesNotMatchValueCaseInsensitively
            = "You can only change lowercase letters to uppercase (or vice versa) when changing the spelling of your email address.";

        public UpdateEmailValueValidator(IQueryEntities entities)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            //EmailAddress email = null;

            RuleFor(p => p.Value)
                // email address cannot be empty
                .NotEmpty()
                    .WithMessage(FailedBecausePreviousSpellingDoesNotMatchValueCaseInsensitively)

                // must be valid against email address regular expression
                .EmailAddress()
                    .WithMessage(FailedBecausePreviousSpellingDoesNotMatchValueCaseInsensitively)

                // validate the number within the Value property b/c remote only validates this property
                .MustFindEmailByNumberAndUserName(entities, x => x.PersonUserName, x => x.Number)
                    .WithMessage(MustFindEmailByNumberAndUserName<object>.FailMessageFormat, x => x.Number, x => x.PersonUserName)
                //.Must((o, p) => ValidateEmailAddress.NumberAndPrincipalMatchesEntity(o.Number, o.PersonUserName.AsPrincipal(), entities, out email))
                //    .WithMessage(ValidateEmailAddress.FailedBecauseNumberAndPrincipalMatchedNoEntity,
                //        p => p.Number, p => p.PersonUserName)

                // must match previous spelling case insensitively
                .MustEqualEmailValueCaseInsensitively(entities, x => x.Number, x => x.PersonUserName)
                    .WithMessage(MustEqualEmailValueCaseInsensitively<object>.FailMessageFormat, x => x.Value)
                //.Must(p => ValidateEmailAddress.NewValueMatchesCurrentValueCaseInsensitively(p, email))
                //    .WithMessage(FailedBecausePreviousSpellingDoesNotMatchValueCaseInsensitively)
            ;
        }
    }

    public static class UpdateEmailValueProfiler
    {
        public class EntityToModelProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<EmailAddress, UpdateEmailValueForm>()
                    .ForMember(d => d.OldSpelling, opt => opt.MapFrom(s => s.Value))
                    .ForMember(d => d.ReturnUrl, opt => opt.Ignore())
                ;
            }
        }

        public class ModelToCommandProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<UpdateEmailValueForm, UpdateMyEmailValue>()
                    .ForMember(d => d.Principal, o => o.Ignore())
                    .ForMember(d => d.NewValue, o => o.MapFrom(s => s.Value))
                    .ForMember(d => d.ChangedState, o => o.Ignore())
                ;
            }
        }
    }
}