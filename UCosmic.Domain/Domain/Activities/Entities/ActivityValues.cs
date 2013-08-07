﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UCosmic.Domain.Activities
{
    public class ActivityValues : RevisableEntity, IEquatable<ActivityValues>
    {
        protected internal ActivityValues()
        {
            Mode = ActivityMode.Draft;
            // ReSharper disable DoNotCallOverridableMethodsInConstructor
            Locations = new Collection<ActivityLocation>();
            Types = new Collection<ActivityType>();
            Tags = new Collection<ActivityTag>();
            Documents = new Collection<ActivityDocument>();
            DateFormat = "MM/dd/yyyy"; // "Custom Date and Time Format Strings" http://msdn.microsoft.com/en-us/library/8kb3ddd4.aspx
            // ReSharper restore DoNotCallOverridableMethodsInConstructor
        }

        public virtual Activity Activity { get; protected internal set; }
        public int ActivityId { get; protected internal set; }

        public string Title { get; protected internal set; }
        public string Content { get; protected internal set; }

        /* Rules for StartsOn and EndsOn:
         * 
         * 1. StartsOn must be equal to or earlier than EndsOn
         * 2. StartsOn and EndsOn may or may not have a value
         * 3. Both are stored in UTC
         */

        private DateTime? _startsOn;
        public DateTime? StartsOn
        {
            get
            {
                return _startsOn.HasValue ? _startsOn.Value.ToLocalTime() : (DateTime?)null;
            }

            protected internal set
            {
                _startsOn = value.HasValue ? value.Value.ToUniversalTime() : (DateTime?)null;
                OrderTime();
            }
        }

        private DateTime? _endsOn;
        public DateTime? EndsOn
        {
            get
            {
                return _endsOn.HasValue ? _endsOn.Value.ToLocalTime() : (DateTime?)null;
            }

            protected internal set
            {
                _endsOn = value.HasValue ? value.Value.ToUniversalTime() : (DateTime?)null;
                OrderTime();
            }
        }

        public bool? OnGoing { get; protected internal set; }
        public string DateFormat { get; protected internal set; }
        public virtual ICollection<ActivityLocation> Locations { get; protected internal set; }
        public virtual ICollection<ActivityType> Types { get; protected internal set; }
        public virtual ICollection<ActivityTag> Tags { get; protected internal set; }
        public virtual ICollection<ActivityDocument> Documents { get; protected internal set; }
        public string ModeText { get; protected set; }
        public ActivityMode Mode
        {
            get { return ModeText.AsEnum<ActivityMode>(); }
            protected internal set { ModeText = value.AsSentenceFragment(); }
        }
        public bool? WasExternallyFunded { get; protected internal set; }
        public bool? WasInternallyFunded { get; protected internal set; }

        private void OrderTime()
        {
            if (_startsOn.HasValue && _endsOn.HasValue)
            {
                if (_startsOn.Value.CompareTo(_endsOn.Value) > 0)
                {
                    DateTime temp = _endsOn.Value;
                    _endsOn = _startsOn.Value;
                    _startsOn = temp;
                }
            }
        }


        public bool IsEmpty()
        {
            bool empty = true;
            empty &= String.IsNullOrEmpty(Title);
            empty &= String.IsNullOrEmpty(Content);
            empty &= !StartsOn.HasValue;
            empty &= !EndsOn.HasValue;
            empty &= !OnGoing.HasValue;
            empty &= Locations == null || Locations.Count == 0;
            empty &= Types == null || Types.Count == 0;
            empty &= Tags == null || Tags.Count == 0;
            empty &= Documents == null || Documents.Count == 0;
            empty &= !WasExternallyFunded.HasValue;
            empty &= !WasInternallyFunded.HasValue;
            return empty;
        }

        protected bool EqualsNullableBool(bool? a, bool? b)
        {
            if (!a.HasValue && !b.HasValue) return true;
            if (!a.HasValue && !b.Value) return true;
            if (!b.HasValue && !a.Value) return true;
            return a == b;
        }

        public bool Equals(ActivityValues other)
        {
            if (other == null) return false;
            bool equal = string.Equals(Title, other.Title);
            equal &= string.Equals(Content, other.Content);
            equal &= StartsOn.Equals(other.StartsOn);
            equal &= EndsOn.Equals(other.EndsOn);
            equal &= EqualsNullableBool(OnGoing, other.OnGoing);
            equal &= string.Equals(DateFormat, other.DateFormat);
            equal &= Locations.OrderBy(a => a.PlaceId).SequenceEqual(other.Locations.OrderBy(b => b.PlaceId));
            equal &= Types.OrderBy(a => a.TypeId).SequenceEqual(other.Types.OrderBy(b => b.TypeId));
            equal &= Tags.OrderBy(a => a.Text).SequenceEqual(other.Tags.OrderBy(b => b.Text));
            equal &= Documents.OrderBy(a => a.Title).SequenceEqual(other.Documents.OrderBy(b => b.Title));
            equal &= string.Equals(ModeText, other.ModeText);
            equal &= EqualsNullableBool(WasExternallyFunded, other.WasExternallyFunded);
            equal &= EqualsNullableBool(WasInternallyFunded, other.WasInternallyFunded);
            return equal;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return ReferenceEquals(this, obj) || Equals(obj as ActivityValues);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Content != null ? Content.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ StartsOn.GetHashCode();
                hashCode = (hashCode * 397) ^ EndsOn.GetHashCode();
                hashCode = (hashCode * 397) ^ OnGoing.GetHashCode();
                hashCode = (hashCode * 397) ^ (DateFormat != null ? DateFormat.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Locations != null ? Locations.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Types != null ? Types.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Tags != null ? Tags.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Documents != null ? Documents.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ModeText != null ? ModeText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ WasExternallyFunded.GetHashCode();
                hashCode = (hashCode * 397) ^ WasInternallyFunded.GetHashCode();
                return hashCode;
            }
        }
    }
}
