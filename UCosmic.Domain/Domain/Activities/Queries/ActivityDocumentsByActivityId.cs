﻿using System;
using System.Linq;

namespace UCosmic.Domain.Activities
{
    public class ActivityDocumentsByActivityId : BaseEntitiesQuery<ActivityDocument>, IDefineQuery<ActivityDocument[]>
    {
        public ActivityDocumentsByActivityId(int activityId)
        {
            ActivityId = activityId;
        }

        public int ActivityId { get; private set; }
        public ActivityMode? Mode { get; set; }
    }

    public class HandleActivityDocumentsByActivityIdQuery : IHandleQueries<ActivityDocumentsByActivityId, ActivityDocument[]>
    {
        private readonly IQueryEntities _entities;

        public HandleActivityDocumentsByActivityIdQuery(IQueryEntities entities)
        {
            _entities = entities;
        }

        public ActivityDocument[] Handle(ActivityDocumentsByActivityId query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var queryable = _entities.Query<ActivityDocument>()
                .EagerLoad(_entities, query.EagerLoad)
                .Where(x => x.ActivityValues.ActivityId == query.ActivityId)
            ;

            // when mode is not requested, assume same mode as activity
            if (!query.Mode.HasValue)
            {
                queryable = queryable.Where(x => x.ActivityValues.ModeText == x.ActivityValues.Activity.ModeText);
            }
            else
            {
                var modeText = query.Mode.Value.AsSentenceFragment();
                queryable = queryable.Where(x => x.ActivityValues.ModeText == modeText);
            }

            queryable = queryable.OrderBy(query.OrderBy);

            return queryable.ToArray();
        }
    }
}
