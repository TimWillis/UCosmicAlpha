﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using FluentValidation;
using UCosmic.Domain.Identity;

namespace UCosmic.Domain.Agreements
{
    public class CreateAgreement
    {
        public CreateAgreement(IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
        }

        public IPrincipal Principal { get; private set; }
        public int? UmbrellaId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public DateTime StartsOn { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpirationEstimated { get; set; }
        public bool? IsAutoRenew { get; set; }
        public string Status { get; set; }
        public string Visibility { get; set; }
    }

    public class ValidateCreateAgreementCommand : AbstractValidator<CreateAgreement>
    {
        public ValidateCreateAgreementCommand(IQueryEntities entities)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            // principal must have name
            RuleFor(x => x.Principal)
                .MustBeInAnyRole(RoleName.AgreementManagers)
            ;

            // umbrella id must exist when passed
            RuleFor(x => x.UmbrellaId)
                .MustFindAgreementById(entities)
                .When(x => x.UmbrellaId.HasValue, ApplyConditionTo.CurrentValidator)
                    .WithMessage(MustFindAgreementById.FailMessageFormat, x => x.UmbrellaId)
            ;

            // type is required
            RuleFor(x => x.Type)
                .NotEmpty()
                    .WithMessage(MustHaveAgreementType.FailMessage)
                .Length(1, AgreementConstraints.TypeMaxLength)
                    .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                        x => "Agreement type", x => AgreementConstraints.TypeMaxLength, x => x.Type.Length)
            ;

            // start date is required
            RuleFor(x => x.StartsOn)
                .NotEmpty()
                    .WithMessage(MustHaveAgreementStartsOn.FailMessage)
                .Must(x => x > DateTime.MinValue)
                    .WithMessage(MustHaveAgreementStartsOn.FailMessage)
            ;

            // expiration date is required
            RuleFor(x => x.ExpiresOn)
                .NotEmpty()
                    .WithMessage(MustHaveAgreementExpiresOn.FailMessage)
                .Must(x => x > DateTime.MinValue)
                    .WithMessage(MustHaveAgreementExpiresOn.FailMessage)
            ;

            // visibility is required
            RuleFor(x => x.Visibility)
                .MustHaveAgreementVisibility()
                    .WithMessage(MustHaveAgreementVisibility.FailMessage)
            ;

            // status is required
            RuleFor(x => x.Status)
                .NotEmpty()
                    .WithMessage(MustHaveAgreementStatus.FailMessage)
                .Length(1, AgreementConstraints.StatusMaxLength)
                    .WithMessage(MustNotExceedStringLength.FailMessageFormat,
                        x => "Agreement status", x => AgreementConstraints.StatusMaxLength, x => x.Status.Length)
            ;

            #region Cheat Sheet

            // text of the establishment name is required, has max length, and must be unique
            //RuleFor(x => x.OfficialName)
            //    .NotNull()
            //        .WithMessage("Establishment must have an official name.")
            //    .Must(x => officialNameValidator.Validate(x).IsValid)
            //        .WithMessage("Establishment official name failed one or more validation rules.")
            //;

            //// validate CEEB code
            //When(x => !string.IsNullOrEmpty(x.CeebCode), () =>
            //    RuleFor(x => x.CeebCode)
            //        .Length(EstablishmentConstraints.CeebCodeLength)
            //            .WithMessage(MustHaveCeebCodeLength.FailMessage)
            //        .MustBeUniqueCeebCode(entities)
            //            .WithMessage(MustBeUniqueCeebCode<object>.FailMessageFormat, x => x.CeebCode)
            //);
            //
            //// validate UCosmic code
            //When(x => !string.IsNullOrEmpty(x.UCosmicCode), () =>
            //    RuleFor(x => x.UCosmicCode)
            //        .Length(EstablishmentConstraints.UCosmicCodeLength)
            //            .WithMessage(MustHaveUCosmicCodeLength.FailMessage)
            //        .MustBeUniqueUCosmicCode(entities)
            //            .WithMessage(MustBeUniqueUCosmicCode<object>.FailMessageFormat, x => x.UCosmicCode)
            //);
            //
            //// when the establishment name is official, it cannot be a former / defunct name
            //When(x => x.OfficialUrl != null && !string.IsNullOrWhiteSpace(x.OfficialUrl.Value), () =>
            //    RuleFor(x => x.OfficialUrl)
            //        .Must(x => officialUrlValidator.Validate(x).IsValid)
            //            .WithMessage("Establishment official URL failed one or more validation rules.")
            //);

            #endregion
        }
    }

    public class HandleCreateAgreementCommand : IHandleCommands<CreateAgreement>
    {
        private readonly ICommandEntities _entities;
        private readonly IUnitOfWork _unitOfWork;
        //private readonly IStoreBinaryData _binaryData;
        //private readonly IHandleCommands<RemoveParticipantFromAgreement> _participantRemover;
        //private readonly IHandleCommands<AddParticipantToAgreement> _participantAdder;
        //private readonly IHandleCommands<RemoveContactFromAgreement> _contactRemover;
        //private readonly IHandleCommands<AddContactToAgreement> _contactAdder;
        //private readonly IHandleCommands<DetachFileFromAgreement> _fileDetacher;
        //private readonly IHandleCommands<AttachFileToAgreement> _fileAttacher;
        private readonly IHandleCommands<UpdateAgreementHierarchy> _hierarchyHandler;

        public HandleCreateAgreementCommand(ICommandEntities entities
            , IUnitOfWork unitOfWork
            //, IStoreBinaryData binaryData
            //, IHandleCommands<RemoveParticipantFromAgreement> participantRemover
            //, IHandleCommands<AddParticipantToAgreement> participantAdder
            //, IHandleCommands<RemoveContactFromAgreement> contactRemover
            //, IHandleCommands<AddContactToAgreement> contactAdder
            //, IHandleCommands<DetachFileFromAgreement> fileDetacher
            //, IHandleCommands<AttachFileToAgreement> fileAttacher
            , IHandleCommands<UpdateAgreementHierarchy> hierarchyHandler
        )
        {
            _entities = entities;
            _unitOfWork = unitOfWork;
            //_binaryData = binaryData;
            //_participantRemover = participantRemover;
            //_participantAdder = participantAdder;
            //_contactRemover = contactRemover;
            //_contactAdder = contactAdder;
            //_fileDetacher = fileDetacher;
            //_fileAttacher = fileAttacher;
            _hierarchyHandler = hierarchyHandler;
        }

        public void Handle(CreateAgreement command)
        {
            
        }

        //public void Handle(CreateAgreement command)
        //{
        //    if (command == null) throw new ArgumentNullException("command");

        //    // start with an agreement entity
        //    var entity = _entities.Get<Agreement>()
        //        .EagerLoad(_entities, new Expression<Func<Agreement, object>>[]
        //        {
        //            a => a.Umbrella,
        //        })
        //        .SingleOrDefault(x => x.Id == command.Id);
        //    if (entity == null && command.Id == 0)
        //        entity = new Agreement();
        //    if (entity == null)
        //        throw new InvalidOperationException(string.Format(
        //            "Agreement with id '{0}' does not exist.", command.Id));

        //    // update scalars
        //    CopyScalars(command, entity);

        //    // scenario 1: no previous umbrella, no current umbrella.
        //    // scenario 2: no previous umbrella, with current umbrella.
        //    // scenario 3: with previous umbrella, same current umbrella.
        //    // scenario 4: with previous umbrella, different current umbrella.
        //    // scenario 5: with previous umbrella, no current umbrella.
        //    var previousUmbrella = entity.Umbrella;
        //    if (command.UmbrellaId.HasValue &&
        //        (previousUmbrella == null || previousUmbrella.Id != command.UmbrellaId.Value))
        //    {
        //        entity.Umbrella = _entities.Get<Agreement>().ById(command.UmbrellaId.Value);
        //        ++command.ChangeCount;
        //    }
        //    else if (previousUmbrella != null && !command.UmbrellaId.HasValue)
        //    {
        //        entity.Umbrella = null;
        //        ++command.ChangeCount;
        //    }

        //    #region Participants

        //    if (command.RemoveParticipantEstablishmentIds != null)
        //        foreach (var removedParticipantEstablishmentId in command.RemoveParticipantEstablishmentIds)
        //        {
        //            var remove = new RemoveParticipantFromAgreement(
        //                command.Principal, removedParticipantEstablishmentId, entity.Id);
        //            _participantRemover.Handle(remove);
        //            if (remove.IsNewlyRemoved) ++command.ChangeCount;
        //        }

        //    if (command.AddParticipantEstablishmentIds != null)
        //        foreach (var addedParticipantEstablishmentId in command.AddParticipantEstablishmentIds)
        //        {
        //            var add = new AddParticipantToAgreement(command.Principal, addedParticipantEstablishmentId, entity);
        //            _participantAdder.Handle(add);
        //            if (add.IsNewlyAdded) ++command.ChangeCount;
        //        }

        //    #endregion
        //    #region Contacts

        //    if (command.RemoveContactIds != null)
        //        foreach (var removedContactEntityId in command.RemoveContactIds.Where(v => v != 0))
        //        {
        //            var remove = new RemoveContactFromAgreement(
        //                command.Principal, removedContactEntityId, entity.Id);
        //            _contactRemover.Handle(remove);
        //            if (remove.IsNewlyRemoved) ++command.ChangeCount;
        //        }

        //    if (command.AddContactCommands != null)
        //        foreach (var add in command.AddContactCommands)
        //        {
        //            add.Agreement = entity;
        //            _contactAdder.Handle(add);
        //            if (add.IsNewlyAdded) ++command.ChangeCount;
        //        }

        //    #endregion
        //    #region Files

        //    if (command.DetachFileIds != null)
        //        foreach (var removedFileEntityId in command.DetachFileIds)
        //        {
        //            var detach = new DetachFileFromAgreement(
        //                command.Principal, removedFileEntityId, entity.Id);
        //            _fileDetacher.Handle(detach);
        //            if (detach.IsNewlyDetached) ++command.ChangeCount;

        //        }

        //    if (command.AttachFileIds != null)
        //        foreach (var attachedFileIds in command.AttachFileIds)
        //        {
        //            var attach = new AttachFileToAgreement(command.Principal, attachedFileIds, entity);
        //            _fileAttacher.Handle(attach);
        //            if (attach.IsNewlyAttached) ++command.ChangeCount;
        //        }

        //    #endregion

        //    command.Id = entity.Id;
        //    if (entity.Id == 0 || command.ChangeCount > 0)
        //    {
        //        if (entity.Id == 0) _entities.Create(entity);
        //        else if (command.ChangeCount > 0) _entities.Update(entity);
        //        DeriveNodes(entity, previousUmbrella);
        //        command.Id = entity.Id;
        //        _unitOfWork.SaveChanges();
        //    }

        //    // fix file path(s)
        //    foreach (var file in entity.Files)
        //    {
        //        var oldPath = file.Path;
        //        var zeroPath = string.Format(AgreementFile.PathFormat, 0, "");
        //        if (oldPath.StartsWith(zeroPath))
        //        {
        //            var guid = oldPath.Substring(zeroPath.Length);
        //            var newPath = string.Format(AgreementFile.PathFormat, entity.Id, guid);
        //            file.Path = newPath;
        //            _unitOfWork.SaveChanges();
        //            _binaryData.Move(oldPath, newPath);
        //        }
        //    }
        //}

        private void DeriveNodes(Agreement agreement, Agreement previousUmbrella)
        {
            _hierarchyHandler.Handle(new UpdateAgreementHierarchy(agreement));
            if (previousUmbrella != null &&
                (agreement.Umbrella == null || agreement.Umbrella.Id != previousUmbrella.Id))
                _hierarchyHandler.Handle(new UpdateAgreementHierarchy(previousUmbrella));
        }

        //private static void CopyScalars(CreateAgreement command, Agreement entity)
        //{
        //    if (command.Title != entity.Title) ++command.ChangeCount;
        //    if (command.IsTitleDerived != entity.IsTitleDerived) ++command.ChangeCount;
        //    if (command.Type != entity.Type) ++command.ChangeCount;
        //    if (command.Status != entity.Status) ++command.ChangeCount;
        //    if (command.Description != entity.Description) ++command.ChangeCount;
        //    if (command.IsAutoRenew != entity.IsAutoRenew) ++command.ChangeCount;
        //    if (command.StartsOn != entity.StartsOn) ++command.ChangeCount;
        //    if (command.ExpiresOn != entity.ExpiresOn) ++command.ChangeCount;
        //    if (command.IsExpirationEstimated != entity.IsExpirationEstimated) ++command.ChangeCount;
        //    if (command.Visibility != entity.Visibility) ++command.ChangeCount;

        //    entity.Title = command.Title;
        //    entity.IsTitleDerived = command.IsTitleDerived;
        //    entity.Type = command.Type;
        //    entity.Status = command.Status;
        //    entity.Description = command.Description;
        //    entity.IsAutoRenew = command.IsAutoRenew;
        //    entity.StartsOn = command.StartsOn;
        //    entity.ExpiresOn = command.ExpiresOn;
        //    entity.IsExpirationEstimated = command.IsExpirationEstimated;
        //    entity.Visibility = command.Visibility;
        //}
    }
}