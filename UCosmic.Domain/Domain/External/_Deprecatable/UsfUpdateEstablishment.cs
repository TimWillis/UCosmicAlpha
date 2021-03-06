﻿//using System;
//using System.Linq;
//using FluentValidation;
//using UCosmic.Domain.Establishments;

//namespace UCosmic.Domain.External
//{
//    public class UsfUpdateEstablishment
//    {
//        //private string _ceebCode;
//        //private string _uCosmicCode;

//        public UsfUpdateEstablishment(int id)
//        {
//            Id = id;
//        }

//        public int Id { get; private set; }

//        //public int? ParentId { get; set; }
//        //public int TypeId { get; set; }
//        //public string CeebCode
//        //{
//        //    get { return _ceebCode; }
//        //    set { _ceebCode = value == null ? null : value.Trim(); }
//        //}
//        //public string UCosmicCode
//        //{
//        //    get { return _uCosmicCode; }
//        //    set { _uCosmicCode = value == null ? null : value.Trim(); }
//        //}

//        public string ExternalId { get; set; }

//        internal bool NoCommit { get; set; }
//    }

//    public class ValidateUsfUpdateEstablishmentCommand : AbstractValidator<UsfUpdateEstablishment>
//    {
//        public ValidateUsfUpdateEstablishmentCommand(IQueryEntities entities)
//        {
//            CascadeMode = CascadeMode.StopOnFirstFailure;

//            // id must be within valid range and exist in the database
//            RuleFor(x => x.Id)
//                .GreaterThanOrEqualTo(1)
//                    .WithMessage(MustBePositivePrimaryKey.FailMessageFormat, x => "Establishment id", x => x.Id)
//                .MustFindEstablishmentById(entities)
//                    .WithMessage(MustFindEstablishmentById.FailMessageFormat, x => x.Id)
//            ;

//            //RuleFor(x => x.ParentId)
//            //    // parent id must exist when passed
//            //    .MustFindEstablishmentById(entities)
//            //    .When(x => x.ParentId.HasValue, ApplyConditionTo.CurrentValidator)
//            //        .WithMessage(MustFindEstablishmentById.FailMessageFormat, x => x.ParentId)

//            //    // cannot have cyclic establishment hierarchies
//            //    .MustNotHaveCyclicHierarchy(entities, x => x.Id)
//            //    .When(x => x.ParentId.HasValue, ApplyConditionTo.CurrentValidator)
//            //;

//            //// type id must exist
//            //RuleFor(x => x.TypeId)
//            //    .MustFindEstablishmentTypeById(entities)
//            //        .WithMessage(MustFindEstablishmentTypeById.FailMessageFormat, x => x.TypeId);

//            //// validate CEEB code
//            //When(x => !string.IsNullOrEmpty(x.CeebCode), () =>
//            //    RuleFor(x => x.CeebCode)
//            //        .Length(EstablishmentConstraints.CeebCodeLength)
//            //            .WithMessage(MustHaveCeebCodeLength.FailMessage)
//            //        .MustBeUniqueCeebCode(entities, x => x.Id)
//            //            .WithMessage(MustBeUniqueCeebCode<object>.FailMessageFormat, x => x.CeebCode)
//            //);

//            //// validate UCosmic code
//            //When(x => !string.IsNullOrEmpty(x.UCosmicCode), () =>
//            //    RuleFor(x => x.UCosmicCode)
//            //        .Length(EstablishmentConstraints.UCosmicCodeLength)
//            //            .WithMessage(MustHaveUCosmicCodeLength.FailMessage)
//            //        .MustBeUniqueUCosmicCode(entities, x => x.Id)
//            //            .WithMessage(MustBeUniqueUCosmicCode<object>.FailMessageFormat, x => x.UCosmicCode)
//            //);
//        }
//    }

//    public class HandleUsfUpdateEstablishmentCommand : IHandleCommands<UsfUpdateEstablishment>
//    {
//        private readonly ICommandEntities _entities;
//        //private readonly IHandleCommands<UsfUpdateEstablishmentHierarchy> _updateHierarchy;
//        private readonly IUnitOfWork _unitOfWork;
//        //private readonly IProcessEvents _eventProcessor;

//        public HandleUsfUpdateEstablishmentCommand(ICommandEntities entities
//            //, IHandleCommands<UsfUpdateEstablishmentHierarchy> updateHierarchy
//            , IUnitOfWork unitOfWork
//            //, IProcessEvents eventProcessor
//        )
//        {
//            _entities = entities;
//            //_updateHierarchy = updateHierarchy;
//            _unitOfWork = unitOfWork;
//            //_eventProcessor = eventProcessor;
//        }

//        public void Handle(UsfUpdateEstablishment command)
//        {
//            if (command == null) throw new ArgumentNullException("command");

//            // load target
//            var entity = _entities.Get<Establishment>()
//                //.EagerLoad(_entities, new Expression<Func<Establishment, object>>[]
//                //{
//                //    x => x.Type.Category,
//                //    x => x.Parent,
//                //})
//                .Single(x => x.RevisionId == command.Id);

//            // only mutate when state is modified
//            //var parentChanged = (command.ParentId.HasValue && entity.Parent == null) ||
//            //    (!command.ParentId.HasValue && entity.Parent != null) ||
//            //    (command.ParentId.HasValue && entity.Parent != null && command.ParentId != entity.Parent.RevisionId);
//            //if (command.TypeId == entity.Type.RevisionId &&
//            //    command.CeebCode == entity.CollegeBoardDesignatedIndicator &&
//            //    command.UCosmicCode == entity.UCosmicCode &&
//            //    !parentChanged
//            //)
//            //    return;

//            // log audit
//            //var audit = new CommandEvent
//            //{
//            //    RaisedBy = command.Principal.Identity.Name,
//            //    Name = command.GetType().FullName,
//            //    Value = JsonConvert.SerializeObject(new
//            //    {
//            //        command.Id,
//            //        command.ParentId,
//            //        command.TypeId,
//            //        command.CeebCode,
//            //        command.UCosmicCode,
//            //        ExternalId = command.ExternalId
//            //    }),
//            //    PreviousState = entity.ToJsonAudit(),
//            //};

//            // update scalars
//            //if (entity.Type.RevisionId != command.TypeId)
//            //{
//            //    var establishmentType = _entities.Get<EstablishmentType>()
//            //        .Single(x => x.RevisionId == command.TypeId);
//            //    entity.Type = establishmentType;
//            //}
//            //entity.CollegeBoardDesignatedIndicator = command.CeebCode;
//            //entity.UCosmicCode = command.UCosmicCode;
//            entity.ExternalId = command.ExternalId;

//            // update parent
//            //if (parentChanged)
//            //    entity.Parent = command.ParentId.HasValue
//            //        ? _entities.Get<Establishment>().Single(x => x.RevisionId == command.ParentId)
//            //        : null;

//            //audit.NewState = entity.ToJsonAudit();
//            //_entities.Create(audit);
//            _entities.Update(entity);

//            // update hierarchy
//            //_updateHierarchy.Handle(new UsfUpdateEstablishmentHierarchy(entity));

//            if (command.NoCommit) return;
//            _unitOfWork.SaveChanges();
//            //_eventProcessor.Raise(new EstablishmentChanged());
//        }
//    }
//}
