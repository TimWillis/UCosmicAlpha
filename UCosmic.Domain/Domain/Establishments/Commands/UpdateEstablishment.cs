﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using FluentValidation;
using Newtonsoft.Json;
using UCosmic.Domain.Audit;

namespace UCosmic.Domain.Establishments
{
    public class UpdateEstablishment
    {
        public UpdateEstablishment(int id, IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
            Id = id;
        }

        public IPrincipal Principal { get; private set; }
        public int Id { get; private set; }

        public int TypeId { get; set; }
        //public string CeebCode { get; set; }
        //public string UCosmicCode { get; set; }

        internal bool NoCommit { get; set; }
    }

    public class ValidateUpdateEstablishmentCommand : AbstractValidator<UpdateEstablishment>
    {
        public ValidateUpdateEstablishmentCommand(IQueryEntities entities)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            // id must be within valid range and exist in the database
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                    .WithMessage(MustBePositivePrimaryKey.FailMessageFormat, x => "Establishment id", x => x.Id)
                .MustFindEstablishmentById(entities)
                    .WithMessage(MustFindEstablishmentById.FailMessageFormat, x => x.Id)
            ;

            // text of the establishment name is required, has max length, and must be unique
            RuleFor(x => x.Principal)
                .Must(x => x.IsInRole(RoleName.EstablishmentAdministrator))
                    .WithMessage("User '{0}' is not authorized to execute this command.")
            ;

            // type id must exist
            RuleFor(x => x.TypeId)
                .MustFindEstablishmentTypeById(entities)
                    .WithMessage(MustFindEstablishmentTypeById.FailMessageFormat, x => x.TypeId);

            //RuleFor(x => x.CeebCode)
            //    .MustBeUniqueCeebCode(entities, x => x.Id)
            //        .WithMessage(MustBeUniqueCeebCode<object>.FailMessageFormat, x => x.CeebCode);

            //RuleFor(x => x.UCosmicCode)
            //    .MustBeUniqueUCosmicCode(entities)
            //        .WithMessage(MustBeUniqueUCosmicCode<object>.FailMessageFormat, x => x.UCosmicCode);
        }
    }

    public class HandleUpdateEstablishmentCommand : IHandleCommands<UpdateEstablishment>
    {
        private readonly ICommandEntities _entities;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProcessEvents _eventProcessor;

        public HandleUpdateEstablishmentCommand(ICommandEntities entities
            , IUnitOfWork unitOfWork
            , IProcessEvents eventProcessor
        )
        {
            _entities = entities;
            _unitOfWork = unitOfWork;
            _eventProcessor = eventProcessor;
        }

        public void Handle(UpdateEstablishment command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // load target
            var entity = _entities.Get<Establishment>()
                .EagerLoad(_entities, new Expression<Func<Establishment, object>>[]
                {
                    x => x.Type.Category,
                })
                .Single(x => x.RevisionId == command.Id);

            // only mutate when state is modified
            if (command.TypeId == entity.Type.RevisionId
                //command.CeebCode == entity.CollegeBoardDesignatedIndicator &&
                //command.UCosmicCode == entity.UCosmicCode
            )
                return;

            // log audit
            var audit = new CommandEvent
            {
                RaisedBy = command.Principal.Identity.Name,
                Name = command.GetType().FullName,
                Value = JsonConvert.SerializeObject(new
                {
                    command.Id,
                    command.TypeId,
                    //command.CeebCode,
                    //command.UCosmicCode,
                }),
                PreviousState = entity.ToJsonAudit(),
            };

            // update scalars
            if (entity.Type.RevisionId != command.TypeId)
            {
                var establishmentType = _entities.Get<EstablishmentType>()
                    .Single(x => x.RevisionId == command.TypeId);
                entity.Type = establishmentType;
            }
            //entity.CollegeBoardDesignatedIndicator = command.CeebCode;
            //entity.UCosmicCode = command.UCosmicCode;

            audit.NewState = entity.ToJsonAudit();
            _entities.Create(audit);
            _entities.Update(entity);

            if (command.NoCommit) return;
            _unitOfWork.SaveChanges();
            _eventProcessor.Raise(new EstablishmentChanged());
        }
    }
}