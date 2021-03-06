﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace UCosmic.Domain.People
{
    public class MustFindEmailByNumberAndUserName<T> : PropertyValidator
    {
        public const string FailMessageFormat = "Email with number '{0}' could not be found for user '{1}'.";

        private readonly IQueryEntities _entities;
        private readonly Func<T, string> _userName;

        internal MustFindEmailByNumberAndUserName(IQueryEntities entities, Func<T, string> userName)
            : base(FailMessageFormat.Replace("{0}", "{PropertyValue}").Replace("{1}", "{UserName}"))
        {
            if (entities == null) throw new ArgumentNullException("entities");
            _entities = entities;
            _userName = userName;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (!(context.PropertyValue is int))
                throw new NotSupportedException(string.Format(
                    "The {0} PropertyValidator can only operate on integer properties", GetType().Name));

            var number = (int)context.PropertyValue;
            var userName = _userName((T)context.Instance);

            context.MessageFormatter.AppendArgument("PropertyValue", userName);
            context.MessageFormatter.AppendArgument("UserName", userName);

            var entity = _entities.Query<EmailAddress>()
                .ByUserNameAndNumber(userName, number);

            return entity != null;
        }
    }

    public static class MustFindEmailByNumberAndUserNameExtensions
    {
        public static IRuleBuilderOptions<T, int> MustFindEmailByNumberAndUserName<T>
            (this IRuleBuilder<T, int> ruleBuilder, IQueryEntities entities, Func<T, string> userName)
        {
            return ruleBuilder.SetValidator(new MustFindEmailByNumberAndUserName<T>(entities, userName));
        }
    }
}
