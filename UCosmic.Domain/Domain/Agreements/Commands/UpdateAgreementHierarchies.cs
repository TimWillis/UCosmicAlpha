﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace UCosmic.Domain.Agreements
{
    public class UpdateAgreementHierarchies
    {
    }

    public class HandleUpdateAgreementHierarchiesCommand : IHandleCommands<UpdateAgreementHierarchies>
    {
        private readonly ICommandEntities _entities;
        private readonly IHandleCommands<UpdateAgreementHierarchy> _hierarchyHandler;

        public HandleUpdateAgreementHierarchiesCommand(ICommandEntities entities
            , IHandleCommands<UpdateAgreementHierarchy> hierarchyHandler
        )
        {
            _entities = entities;
            _hierarchyHandler = hierarchyHandler;
        }

        // when creating a new agreement, there will be no children.
        // however, there may be an umbrella and other ancestors.
        // each ancestor must have its offspring and children updated.
        // when changing an existing agreement, there may be ancestors and offspring.
        // however, they only change when the agreement's umbrella has changed.
        // each ancestor in the old tree must have its offspring and children updated.
        // each ancestor and offspring in the new tree must have its offspring and ancestors updated.
        public void Handle(UpdateAgreementHierarchies command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var agreements = _entities.Get<Agreement>()
                .EagerLoad(_entities, new Expression<Func<Agreement, object>>[]
                {
                    e => e.Offspring.Select(o => o.Ancestor.Umbrella),
                    e => e.Offspring.Select(o => o.Offspring.Umbrella),
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
            foreach (var parent in agreements)
                _hierarchyHandler.Handle(new UpdateAgreementHierarchy(parent));
        }
    }
}
