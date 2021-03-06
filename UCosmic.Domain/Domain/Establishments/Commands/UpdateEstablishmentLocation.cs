﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;
using FluentValidation;
using Newtonsoft.Json;
using UCosmic.Domain.Audit;
using UCosmic.Domain.Places;

namespace UCosmic.Domain.Establishments
{
    public class UpdateEstablishmentLocation
    {
        public UpdateEstablishmentLocation(int id, IPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException("principal");
            Principal = principal;
            Id = id;
        }

        public IPrincipal Principal { get; private set; }
        public int? Id { get; internal set; }
        internal EstablishmentLocation Entity { get; set; }

        public double? CenterLatitude { get; set; }
        public double? CenterLongitude { get; set; }
        public double? BoxNorthEastLatitude { get; set; }
        public double? BoxNorthEastLongitude { get; set; }
        public double? BoxSouthWestLatitude { get; set; }
        public double? BoxSouthWestLongitude { get; set; }
        public int? GoogleMapZoomLevel { get; set; }
        public int? PlaceId { get; set; }

        internal bool NoCommit { get; set; }
    }

    public class ValidateUpdateEstablishmentLocationCommand : AbstractValidator<UpdateEstablishmentLocation>
    {
        public ValidateUpdateEstablishmentLocationCommand(IQueryEntities entities)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            When(x => x.Entity == null, () =>
                RuleFor(x => x.Id.HasValue).Equal(true).WithMessage("Establishment Location owner id must be provided.")
            );

            // id must be within valid range and exist in the database
            When(x => x.Id.HasValue, () =>
                RuleFor(x => x.Id.Value)
                    .GreaterThanOrEqualTo(1)
                        .WithMessage(MustBePositivePrimaryKey.FailMessageFormat, x => "Establishment id", x => x.Id)
                    .MustFindEstablishmentById(entities)
                        .WithMessage(MustFindEstablishmentById.FailMessageFormat, x => x.Id)
            );

            // text of the establishment name is required, has max length, and must be unique
            //RuleFor(x => x.Principal)
            //    .Must(x => x.IsInRole(RoleName.EstablishmentAdministrator))
            //        .WithMessage("User '{0}' is not authorized to execute this command.", x => x.Principal.Identity.Name)
            //;

            // place must exist if specified
            When(x => x.PlaceId.HasValue, () =>
                RuleFor(x => x.PlaceId.Value)
                    .MustFindPlaceById(entities)
            );
        }
    }

    public class HandleUpdateEstablishmentLocationCommand : IHandleCommands<UpdateEstablishmentLocation>
    {
        private readonly ICommandEntities _entities;
        private readonly ITriggerEvent<EstablishmentChanged> _eventTrigger;

        public HandleUpdateEstablishmentLocationCommand(ICommandEntities entities
            , ITriggerEvent<EstablishmentChanged> eventTrigger
        )
        {
            _entities = entities;
            _eventTrigger = eventTrigger;
        }

        public void Handle(UpdateEstablishmentLocation command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // load target
            var entity = command.Entity ?? _entities.Get<EstablishmentLocation>()
                .EagerLoad(_entities, new Expression<Func<EstablishmentLocation, object>>[]
                {
                    x => x.Places,
                })
                .Single(x => x.RevisionId == command.Id.Value);

            // load place
            var commandPlaces = new List<Place>();
            if (command.PlaceId.HasValue)
            {
                var commandPlace = _entities.Get<Place>()
                    .EagerLoad(_entities, new Expression<Func<Place, object>>[]
                    {
                        x => x.Ancestors.Select(y => y.Ancestor),
                    })
                    .Single(x => x.RevisionId == command.PlaceId.Value);
                commandPlaces = commandPlace.Ancestors.OrderByDescending(x => x.Separation)
                    .Select(x => x.Ancestor).ToList();
                commandPlaces.Add(commandPlace);
            }

            // only mutate when state is modified
            if (command.CenterLatitude == entity.Center.Latitude &&
                command.CenterLongitude == entity.Center.Longitude &&
                command.BoxNorthEastLatitude == entity.BoundingBox.Northeast.Latitude &&
                command.BoxNorthEastLongitude == entity.BoundingBox.Northeast.Longitude &&
                command.BoxSouthWestLatitude == entity.BoundingBox.Southwest.Latitude &&
                command.BoxSouthWestLongitude == entity.BoundingBox.Southwest.Longitude &&
                commandPlaces.Select(x => x.RevisionId).SequenceEqual(entity.Places.Select(x => x.RevisionId)) &&
                command.GoogleMapZoomLevel == entity.GoogleMapZoomLevel
            )
                return;

            // log audit
            var audit = new CommandEvent
            {
                RaisedBy = command.Principal.Identity.Name,
                Name = command.GetType().FullName,
                Value = JsonConvert.SerializeObject(new
                {
                    Id = command.Entity != null ? command.Entity.RevisionId : command.Id,
                    command.CenterLatitude,
                    command.CenterLongitude,
                    command.BoxNorthEastLatitude,
                    command.BoxNorthEastLongitude,
                    command.BoxSouthWestLatitude,
                    command.BoxSouthWestLongitude,
                    command.PlaceId
                }),
                PreviousState = entity.ToJsonAudit(),
            };

            // update scalars
            entity.Center = new Coordinates(command.CenterLatitude, command.CenterLongitude);
            entity.BoundingBox = new BoundingBox(command.BoxNorthEastLatitude, command.BoxNorthEastLongitude,
                command.BoxSouthWestLatitude, command.BoxSouthWestLongitude);
            entity.GoogleMapZoomLevel = command.GoogleMapZoomLevel;

            // update places
            if (command.PlaceId.HasValue)
                entity.Places = commandPlaces;
            else
                entity.Places = new Collection<Place>();

            audit.NewState = entity.ToJsonAudit();
            _entities.Create(audit);
            if (entity.RevisionId != default(int))
                _entities.Update(entity);

            if (command.NoCommit) return;
            _entities.SaveChanges();
            _eventTrigger.Raise(new EstablishmentChanged());
        }
    }
}
