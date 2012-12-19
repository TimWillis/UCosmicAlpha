﻿using System.Data.Entity.ModelConfiguration;
using UCosmic.Domain.Activities;

namespace UCosmic.EntityFramework
{
    public class ActivityOrm : EntityTypeConfiguration<Activity>
    {
        public ActivityOrm()
        {
            ToTable(typeof(Activity).Name, DbSchemaName.Activities);

            HasKey(p => new { p.PersonId, p.Number });

            HasRequired(d => d.Person)
                .WithMany()
                .HasForeignKey(d => d.PersonId)
                .WillCascadeOnDelete(true);

            Property(p => p.ModeText).HasColumnName("Mode").IsRequired().HasMaxLength(20);
            Property(p => p.Values.Title).HasColumnName("Title").HasMaxLength(200);
            Property(p => p.Values.Content).HasColumnName("Content").HasColumnType("ntext");
            Property(p => p.Values.StartsOn).HasColumnName("StartsOn");
            Property(p => p.Values.EndsOn).HasColumnName("EndsOn");
            Property(p => p.DraftedValues.Title).HasColumnName("DraftedTitle").HasMaxLength(200);
            Property(p => p.DraftedValues.Content).HasColumnName("DraftedContent").HasColumnType("ntext");
            Property(p => p.DraftedValues.StartsOn).HasColumnName("DraftedStartsOn");
            Property(p => p.DraftedValues.EndsOn).HasColumnName("DraftedEndsOn");
            Ignore(p => p.Mode);
        }
    }

    public class ActivityTagOrm : EntityTypeConfiguration<ActivityTag>
    {
        public ActivityTagOrm()
        {
            ToTable(typeof(ActivityTag).Name, DbSchemaName.Activities);

            HasKey(p => new { p.ActivityPersonId, p.ActivityNumber, p.Number });

            HasRequired(d => d.Activity)
                .WithMany(p => p.Tags)
                .HasForeignKey(d => new { d.ActivityPersonId, d.ActivityNumber })
                .WillCascadeOnDelete(true);

            Property(p => p.DomainTypeText).HasColumnName("DomainType").IsRequired().HasMaxLength(20);
            Property(p => p.Text).IsRequired().HasMaxLength(500);
            Ignore(p => p.DomainType);
        }
    }

    public class DraftedTagOrm : EntityTypeConfiguration<DraftedTag>
    {
        public DraftedTagOrm()
        {
            ToTable(typeof(DraftedTag).Name, DbSchemaName.Activities);

            HasKey(p => new { p.ActivityPersonId, p.ActivityNumber, p.Number });

            HasRequired(d => d.Activity)
                .WithMany(p => p.DraftedTags)
                .HasForeignKey(d => new { d.ActivityPersonId, d.ActivityNumber })
                .WillCascadeOnDelete(true);

            Property(p => p.DomainTypeText).HasColumnName("DomainType").IsRequired().HasMaxLength(20);
            Property(p => p.Text).IsRequired().HasMaxLength(500);
            Ignore(p => p.DomainType);
        }
    }
}
