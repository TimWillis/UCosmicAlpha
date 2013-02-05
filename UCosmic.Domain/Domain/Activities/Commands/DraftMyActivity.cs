﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Principal;

namespace UCosmic.Domain.Activities
{
    public class DraftMyActivity
    {
        public IPrincipal Principal { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? StartsOn { get; set; }
        public DateTime? EndsOn { get; set; }
        public ActivityCommandTag[] Tags { get; set; }
    }

    public class ActivityCommandTag
    {
        public string Text { get; set; }
        public ActivityTagDomainType DomainType { get; set; }
        public int? DomainKey { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class HandleDraftMyActivityCommand : IHandleCommands<DraftMyActivity>
    {
        private readonly ICommandEntities _entities;

        public HandleDraftMyActivityCommand(ICommandEntities entities)
        {
            _entities = entities;
        }

        public void Handle(DraftMyActivity command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var activity = _entities.Get<Activity>()
                .EagerLoad(_entities, new Expression<Func<Activity, object>>[]
                {
                    t => t.DraftedTags,
                })
                .ByUserNameAndNumber(command.Principal.Identity.Name, command.Number);
            if (activity == null) return;

            activity.DraftedValues.Title = command.Title;
            activity.DraftedValues.Content = command.Content;
            activity.DraftedValues.StartsOn = command.StartsOn;
            activity.DraftedValues.EndsOn = command.EndsOn;

            if (command.Tags != null)
            {
                // remove deleted tags
                foreach (var tagToDelete in command.Tags.Where(t => t.IsDeleted)
                    .Select(deletedTag => activity.DraftedTags
                        .Where(
                            draftedTag =>
                            draftedTag.Text == deletedTag.Text &&
                            draftedTag.DomainType == deletedTag.DomainType &&
                            draftedTag.DomainKey == deletedTag.DomainKey
                        ).ToArray()
                    )
                    .SelectMany(tagsToDelete => tagsToDelete)
                )
                {
                    activity.DraftedTags.Remove(tagToDelete);
                }

                // add new tags
                foreach (var tagToAddOrKeep in
                    from tagToAddOrKeep in command.Tags.Where(t => !t.IsDeleted)
                    let draftedTag = activity.DraftedTags
                        .Where(
                            t =>
                            t.Text == tagToAddOrKeep.Text &&
                            t.DomainType == tagToAddOrKeep.DomainType &&
                            t.DomainKey == tagToAddOrKeep.DomainKey
                        ).ToArray()
                    where !draftedTag.Any()
                    select tagToAddOrKeep)
                {
                    activity.DraftedTags.Add(new DraftedTag
                    {
                        Activity = activity,
                        ActivityNumber = activity.Number,
                        ActivityPersonId = activity.PersonId,
                        Number = activity.DraftedTags.NextNumber(),
                        Text = tagToAddOrKeep.Text,
                        DomainType = tagToAddOrKeep.DomainType,
                        DomainKey = tagToAddOrKeep.DomainKey,
                    });
                }
            }

            activity.UpdatedOn = DateTime.UtcNow;

            _entities.Update(activity);
        }
    }
}
