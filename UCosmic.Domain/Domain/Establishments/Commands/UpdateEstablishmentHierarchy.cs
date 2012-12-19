﻿using System;
using System.Linq;

namespace UCosmic.Domain.Establishments
{
    public class UpdateEstablishmentHierarchy
    {
        public UpdateEstablishmentHierarchy(Establishment establishment)
        {
            if (establishment == null) throw new ArgumentNullException("establishment");
            Establishment = establishment;
        }

        public Establishment Establishment { get; private set; }
    }

    public class HandleUpdateEstablishmentHierarchyCommand : IHandleCommands<UpdateEstablishmentHierarchy>
    {
        private readonly ICommandEntities _entities;

        public HandleUpdateEstablishmentHierarchyCommand(ICommandEntities entities)
        {
            _entities = entities;
        }

        public void Handle(UpdateEstablishmentHierarchy command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var parent = command.Establishment;
            while (parent.Parent != null)
                parent = parent.Parent;

            ClearNodesRecursive(parent);
            BuildNodesRecursive(parent);
        }

        private void ClearNodesRecursive(Establishment parent)
        {
            // delete all of this parent's offspring nodes
            while (parent.Offspring.FirstOrDefault() != null)
                _entities.Purge(parent.Offspring.First());

            // operate recursively over children
            foreach (var child in parent.Children)
            {
                // delete each of the child's ancestor nodes
                while (child.Ancestors.FirstOrDefault() != null)
                    _entities.Purge(child.Ancestors.First());

                // run this method again on the child
                ClearNodesRecursive(child);
            }
        }

        private static void BuildNodesRecursive(Establishment parent)
        {
            // operate recursively over children
            foreach (var child in parent.Children)
            {
                // create & add ancestor node for this child
                var node = new EstablishmentNode
                {
                    Ancestor = parent,
                    Offspring = child,
                    Separation = 1,
                };
                child.Ancestors.Add(node);

                // loop over the parent's ancestors
                foreach (var ancestor in parent.Ancestors)
                {
                    // create & add ancestor node for this child
                    node = new EstablishmentNode
                    {
                        Ancestor = ancestor.Ancestor,
                        Offspring = child,
                        Separation = ancestor.Separation + 1,
                    };
                    child.Ancestors.Add(node);
                }

                // run this method again on the child
                BuildNodesRecursive(child);
            }
        }
    }
}
