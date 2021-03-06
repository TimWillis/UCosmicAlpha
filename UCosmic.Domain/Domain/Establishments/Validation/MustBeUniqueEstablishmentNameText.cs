﻿using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace UCosmic.Domain.Establishments
{
    public class MustBeUniqueEstablishmentNameText<T> : PropertyValidator
    {
        public const string FailMessageFormat = "The establishment name '{0}' already exists.";

        private readonly IQueryEntities _entities;
        private readonly Func<T, int> _ownId;

        internal MustBeUniqueEstablishmentNameText(IQueryEntities entities, Func<T, int> ownId)
            : base(FailMessageFormat.Replace("{0}", "{PropertyValue}"))
        {
            if (entities == null) throw new ArgumentNullException("entities");
            _entities = entities;
            _ownId = ownId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (!(context.PropertyValue is string))
                throw new NotSupportedException(string.Format(
                    "The {0} PropertyValidator can only operate on string properties", GetType().Name));

            context.MessageFormatter.AppendArgument("PropertyValue", context.PropertyValue);
            var value = (string)context.PropertyValue;
            var ownId = _ownId != null ? _ownId((T)context.Instance) : (int?) null;

            var entity = _entities.Query<EstablishmentName>()
                .FirstOrDefault(
                    x =>
                    (ownId.HasValue ? x.RevisionId != ownId.Value : x.RevisionId != 0) &&
                    (x.Text.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                    (x.AsciiEquivalent != null && x.AsciiEquivalent.Equals(value, StringComparison.OrdinalIgnoreCase)))
                );

            return entity == null;
        }
    }

    public static class MustBeUniqueEstablishmentNameTextExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeUniqueEstablishmentNameText<T>
            (this IRuleBuilder<T, string> ruleBuilder, IQueryEntities entities, Func<T, int> ownId = null)
        {
            return ruleBuilder.SetValidator(new MustBeUniqueEstablishmentNameText<T>(entities, ownId));
        }
    }
}
