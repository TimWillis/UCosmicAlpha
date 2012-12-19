﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace UCosmic.Domain.Establishments
{
    public class UpdateEstablishmentHierarchies
    {
    }

    public class HandleUpdateEstablishmentHierarchiesCommand : IHandleCommands<UpdateEstablishmentHierarchies>
    {
        private readonly ICommandEntities _entities;
        private readonly IHandleCommands<UpdateEstablishmentHierarchy> _hierarchyHandler;

        public HandleUpdateEstablishmentHierarchiesCommand(ICommandEntities entities
            , IHandleCommands<UpdateEstablishmentHierarchy> hierarchyHandler
        )
        {
            _entities = entities;
            _hierarchyHandler = hierarchyHandler;
        }

        public void Handle(UpdateEstablishmentHierarchies command)
        {
            if (command == null) throw new ArgumentNullException("command");

            // get all root-level establishments with children
            var establishments = _entities.Get<Establishment>()
                .EagerLoad(_entities, new Expression<Func<Establishment, object>>[]
                {
                    e => e.Offspring.Select(o => o.Ancestor.Parent),
                    e => e.Offspring.Select(o => o.Offspring.Parent),
                    e => e.Offspring.Select(o => o.Ancestor.Children),
                    e => e.Offspring.Select(o => o.Offspring.Children),
                    e => e.Children.Select(c => c.Children.Select(g => g.Children)),
                    e => e.Children.Select(c => c.Ancestors.Select(a => a.Ancestor))
                })
                .IsRoot()
                .WithAnyChildren()
                .ToArray()
            ;

            // derive nodes for each parent
            foreach (var parent in establishments)
                _hierarchyHandler.Handle(new UpdateEstablishmentHierarchy(parent));
        }
    }
}
