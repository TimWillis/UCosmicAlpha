﻿using System;
using System.Linq;
using System.Linq.Expressions;
using FluentValidation;
using UCosmic.Domain.Languages;

namespace UCosmic.Domain.Establishments
{
    public class UpdateEstablishmentName
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsOfficialName { get; set; }
        public bool IsFormerName { get; set; }
        public string LanguageCode { get; set; }
    }

    public class ValidateUpdateEstablishmentNameCommand : AbstractValidator<UpdateEstablishmentName>
    {
        private readonly IQueryEntities _entities;
        private EstablishmentName _establishmentName;

        public ValidateUpdateEstablishmentNameCommand(IQueryEntities entities)
        {
            _entities = entities;

            RuleFor(x => x.Id)
                // id must be within valid range
                .GreaterThanOrEqualTo(1)
                .WithMessage("Establishment name id '{0}' is not valid.", x => x.Id)

                // id must exist in the database
                .Must(Exist)
                .WithMessage("Establishment name with id '{0}' does not exist", x => x.Id)
            ;

            // text of the establishment name is required and has max length
            RuleFor(x => x.Text)
                .NotEmpty()
                    .WithMessage("Establishment name is required.")
                .Length(1, 400)
                    .WithMessage("Establishment name cannot exceed 400 characters. You entered {0} character{1}.",
                        name => name.Text.Length,
                        name => name.Text.Length == 1 ? "" : "s")
            ;

            // when the establishment name is official, it cannot be a former / defunct name
            When(x => x.IsOfficialName, () =>
                RuleFor(x => x.IsFormerName).Equal(false)
                .WithMessage("An official establishment name cannot also be a former or defunct name.")
            );
        }

        private bool Exist(int id)
        {
            _establishmentName = _entities.Query<EstablishmentName>()
                .SingleOrDefault(y => y.RevisionId == id)
            ;
            return _establishmentName != null;
        }
    }

    public class HandleUpdateEstablishmentNameCommand: IHandleCommands<UpdateEstablishmentName>
    {
        private readonly ICommandEntities _entities;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProcessEvents _eventProcessor;

        public HandleUpdateEstablishmentNameCommand(ICommandEntities entities, IUnitOfWork unitOfWork, IProcessEvents eventProcessor)
        {
            _entities = entities;
            _unitOfWork = unitOfWork;
            _eventProcessor = eventProcessor;
        }

        public void Handle(UpdateEstablishmentName command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // eager load related entities
            var entity = _entities.Get<EstablishmentName>()
                .EagerLoad(_entities, new Expression<Func<EstablishmentName, object>>[]
                {
                    x => x.ForEstablishment.Names, // parent & siblings
                    x => x.TranslationToLanguage, // language
                })
                .Single(x => x.RevisionId == command.Id);

            // get new language
            var language = _entities.Get<Language>()
                .SingleOrDefault(x =>  x.TwoLetterIsoCode.Equals(command.LanguageCode, StringComparison.OrdinalIgnoreCase));

            entity.Text = command.Text;
            entity.IsFormerName = command.IsFormerName;
            entity.IsOfficialName = command.IsOfficialName;
            entity.TranslationToLanguage = language;

            // can only be one official name
            if (command.IsOfficialName)
            {
                foreach (var name in entity.ForEstablishment.Names)
                {
                    name.IsOfficialName = false;
                    _entities.Update(name);
                }
                entity.ForEstablishment.OfficialName = command.Text;
                entity.IsOfficialName = true;
                entity.IsFormerName = false; // official name cannot be defunct
                _entities.Update(entity.ForEstablishment);
            }

            _entities.Update(entity);
            _unitOfWork.SaveChanges();
            _eventProcessor.Raise(new EstablishmentChanged());
        }
    }
}