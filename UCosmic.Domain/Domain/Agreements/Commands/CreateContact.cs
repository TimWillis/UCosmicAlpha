﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using FluentValidation;
using Newtonsoft.Json;
using UCosmic.Domain.Audit;
using UCosmic.Domain.Identity;
using UCosmic.Domain.People;

namespace UCosmic.Domain.Agreements
{
    public class CreateContact
    {
        public CreateContact(IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
        public int AgreementId { get; set; }
        public string Type { get; set; }
        public int? PersonId { get; set; }
        public string Salutation { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string EmailAddress { get; set; }
        public string JobTitle { get; set; }
        public IEnumerable<CreateContactPhone> Phones { get; set; }

        public int CreatedContactId { get; internal set; }
    }

    public class ValidateCreateContactCommand : AbstractValidator<CreateContact>
    {
        public ValidateCreateContactCommand(IQueryEntities entities, IProcessQueries queryProcessor)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            // principal must be authorized to perform this action
            RuleFor(x => x.Principal)
                .NotNull()
                    .WithMessage(MustNotHaveNullPrincipal.FailMessage)
                .MustNotHaveEmptyIdentityName()
                    .WithMessage(MustNotHaveEmptyIdentityName.FailMessage)
                .MustBeInAnyRole(RoleName.AgreementManagers)
            ;

            RuleFor(x => x.AgreementId)
                // agreement id must exist
                .MustFindAgreementById(entities)
                    .WithMessage(MustFindAgreementById<object>.FailMessageFormat, x => x.AgreementId)

                // principal must own agreement
                .MustBeOwnedByPrincipal(queryProcessor, x => x.Principal)
                    .WithMessage(MustBeOwnedByPrincipal<object>.FailMessageFormat, x => x.AgreementId, x => x.Principal.Identity.Name)
            ;

            // type is required
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage(MustHaveContactType.FailMessage)
                .Length(1, AgreementContactConstraints.TypeMaxLength)
                    .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                        x => "Contact type", x => AgreementContactConstraints.TypeMaxLength, x => x.Type.Length)
            ;

            // when personid is present
            When(x => x.PersonId.HasValue, () =>
                // the person must exist in the database
                RuleFor(x => x.PersonId.Value)
                    .MustFindPersonById(entities)
                        .WithMessage(MustFindPersonById.FailMessageFormat, x => x.PersonId)
            );

            // when personid is notpresent
            When(x => !x.PersonId.HasValue, () =>
            {
                // first and last name are required
                RuleFor(x => x.FirstName).NotEmpty().WithMessage(MustHaveFirstName.FailMessage);
                RuleFor(x => x.LastName).NotEmpty().WithMessage(MustHaveLastName.FailMessage);

                // other name fields cannot exceed length
                When(x => !string.IsNullOrWhiteSpace(x.Salutation), () => 
                    RuleFor(x => x.Salutation)
                        .Length(1, PersonConstraints.SalutationMaxLength)
                            .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                                x => "Salutation", x => PersonConstraints.SalutationMaxLength, x => x.Salutation.Length)
                );
                When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () =>
                    RuleFor(x => x.MiddleName)
                        .Length(1, PersonConstraints.MiddleNameMaxLength)
                            .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                                x => "Middle name", x => PersonConstraints.MiddleNameMaxLength, x => x.MiddleName.Length)
                );
                When(x => !string.IsNullOrWhiteSpace(x.Suffix), () =>
                    RuleFor(x => x.Suffix)
                        .Length(1, PersonConstraints.SuffixMaxLength)
                            .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                                x => "Suffix", x => PersonConstraints.SuffixMaxLength, x => x.Suffix.Length)
                );
                When(x => !string.IsNullOrWhiteSpace(x.EmailAddress), () =>
                    RuleFor(x => x.EmailAddress)
                        .EmailAddress()
                            .WithMessage(MustBeValidEmailAddressFormat.FailMessageFormat,
                                x => x.EmailAddress)
                        .Length(1, EmailAddressConstraints.ValueMaxLength)
                            .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                                x => "Email address", x => EmailAddressConstraints.ValueMaxLength, x => x.EmailAddress.Length)
                );
            });

            // title must meet length requirements
            When(x => !string.IsNullOrWhiteSpace(x.JobTitle), () =>
                RuleFor(x => x.JobTitle)
                    .Length(1, AgreementContactConstraints.TitleMaxLength)
                        .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                            x => "Job title", x => AgreementContactConstraints.TitleMaxLength, x => x.JobTitle.Length)
            );
        }
    }

    public class HandleCreateContactCommand : IHandleCommands<CreateContact>
    {
        private readonly ICommandEntities _entities;
        private readonly IHandleCommands<CreatePerson> _createPerson;
        private readonly IHandleCommands<CreateEmailAddress> _createEmailAddress;
        private readonly IHandleCommands<CreateContactPhone> _createPhone;
        private readonly IUnitOfWork _unitOfWork;

        public HandleCreateContactCommand(ICommandEntities entities
            , IHandleCommands<CreatePerson> createPerson
            , IHandleCommands<CreateEmailAddress> createEmailAddress
            , IHandleCommands<CreateContactPhone> createPhone
            , IUnitOfWork unitOfWork
        )
        {
            _entities = entities;
            _createPerson = createPerson;
            _createEmailAddress = createEmailAddress;
            _createPhone = createPhone;
            _unitOfWork = unitOfWork;
        }

        public void Handle(CreateContact command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // are we attaching to existing person, or creating a new one?
            var person = command.PersonId.HasValue
                ? _entities.Get<Person>().Single(x => x.RevisionId == command.PersonId.Value) : null;
            if (person == null)
            {
                // create person entity
                var createPersonCommand = new CreatePerson
                {
                    Salutation = command.Salutation,
                    FirstName = command.FirstName,
                    MiddleName = command.MiddleName,
                    LastName = command.LastName,
                    Suffix = command.Suffix,
                    NoCommit = true,
                };
                _createPerson.Handle(createPersonCommand);
                person = createPersonCommand.CreatedPerson;

                // attach email address if provided
                if (!string.IsNullOrWhiteSpace(command.EmailAddress))
                {
                    _createEmailAddress.Handle(new CreateEmailAddress(command.EmailAddress, person)
                    {
                        IsDefault = true,
                        NoCommit = true,
                    });
                }
            }

            var entity = new AgreementContact
            {
                AgreementId = command.AgreementId,
                Person = person,
                Type = command.Type,
                Title = command.JobTitle,
            };
            _entities.Create(entity);

            if (command.Phones != null)
                foreach (var phone in command.Phones)
                    _createPhone.Handle(new CreateContactPhone(command.Principal, entity)
                    {
                        NoCommit = true,
                        Type = phone.Type,
                        Value = phone.Value,
                    });

            // log audit
            var audit = new CommandEvent
            {
                RaisedBy = command.Principal.Identity.Name,
                Name = command.GetType().FullName,
                Value = JsonConvert.SerializeObject(new
                {
                    command.AgreementId,
                    command.Type,
                    command.PersonId,
                    command.Salutation,
                    command.FirstName,
                    command.MiddleName,
                    command.LastName,
                    command.Suffix,
                    command.EmailAddress,
                    command.JobTitle,
                }),
                NewState = entity.ToJsonAudit(),
            };
            _entities.Create(audit);

            _unitOfWork.SaveChanges();
            command.CreatedContactId = entity.Id;
        }
    }
}
