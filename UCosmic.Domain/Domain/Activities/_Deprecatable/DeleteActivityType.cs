﻿//using System;
//using System.Linq;
//using System.Security.Principal;
//using FluentValidation;

//namespace UCosmic.Domain.Activities
//{
//    public class DeleteActivityType
//    {
//        public IPrincipal Principal { get; private set; }
//        public int Id { get; private set; }
//        internal bool NoCommit { get; set; }

//        public DeleteActivityType(IPrincipal principal, int id)
//        {
//            if (principal == null) { throw new ArgumentNullException("principal"); }
//            Principal = principal;
//            Id = id;
//        }
//    }

//    public class ValidateDeleteActivityTypeCommand : AbstractValidator<DeleteActivityType>
//    {
//        public ValidateDeleteActivityTypeCommand(IQueryEntities entities)
//        {
//            CascadeMode = CascadeMode.StopOnFirstFailure;

//            RuleFor(x => x.Principal)
//                .MustOwnActivityType(entities, x => x.Id)
//                .WithMessage(MustOwnActivityType<object>.FailMessageFormat, x => x.Principal.Identity.Name, x => x.Id);

//            RuleFor(x => x.Id)
//                // id must be within valid range
//                .GreaterThanOrEqualTo(1)
//                    .WithMessage(MustBePositivePrimaryKey.FailMessageFormat, x => "ActivityType id", x => x.Id)
//            ;
//        }
//    }

//    public class HandleDeleteActivityTypeCommand : IHandleCommands<DeleteActivityType>
//    {
//        private readonly ICommandEntities _entities;
//        private readonly IUnitOfWork _unitOfWork;
//        //private readonly IProcessEvents _eventProcessor;

//        public HandleDeleteActivityTypeCommand(ICommandEntities entities
//            , IUnitOfWork unitOfWork
//            //, IProcessEvents eventProcessor
//        )
//        {
//            _entities = entities;
//            _unitOfWork = unitOfWork;
//            //_eventProcessor = eventProcessor;
//        }

//        public void Handle(DeleteActivityType command)
//        {
//            if (command == null) throw new ArgumentNullException("command");

//            // load target
//            var activityType = _entities.Get<ActivityType>().SingleOrDefault(x => x.RevisionId == command.Id);
//            if (activityType == null) return; // delete idempotently

//            // TBD
//            // log audit
//            //var audit = new CommandEvent
//            //{
//            //    RaisedBy = User.Name,
//            //    Name = command.GetType().FullName,
//            //    Value = JsonConvert.SerializeObject(new { command.Id }),
//            //    PreviousState = activityDocument.ToJsonAudit(),
//            //};

//            //_entities.Create(audit);
//            _entities.Purge(activityType);

//            if (!command.NoCommit)
//            {
//                _unitOfWork.SaveChanges();
//            }

//            //_eventProcessor.Raise(new EstablishmentChanged());
//        }
//    }
//}
