﻿using System.Data.Entity.ModelConfiguration;
using UCosmic.Domain.Representatives;

namespace UCosmic.EntityFramework
{
    public class RepModuleSettingsOrm : EntityTypeConfiguration<RepModuleSettings>
    {
        public RepModuleSettingsOrm()
        {
            ToTable(typeof(RepModuleSettings).Name, DbSchemaName.Representatives);

            HasRequired(d => d.Owner)
                .WithOptional()
                .Map(m => m.MapKey("OwnerId"))
                .WillCascadeOnDelete(true)
            ;

            Property(p => p.WelcomeMessage).HasColumnType("ntext").IsOptional();
        }
    }
}