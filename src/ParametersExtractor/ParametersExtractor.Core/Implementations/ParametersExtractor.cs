using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ParametersExtractor.Core.Implementations
{
    public sealed class ParametersExtractor<TObject> : IParametersExtractor<TObject> where TObject : class
    {
        private readonly TObject _object;

        private readonly Dictionary<string, object> _paramters = new Dictionary<string, object>();

        public ParametersExtractor(TObject @object)
        {
            _object = @object;
        }

        public IParametersExtractor<TObject> Extract<TParameter>(string name, Func<TObject, TParameter> function)
        {
            _paramters[name] = function(_object);

            return this;
        }

        public IParametersExtractor<TObject> Extract<TParameter>(Expression<Func<TObject, TParameter>> expression)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            _paramters[name] = GetParameterValue(expression);

            return this;
        }

        public IParametersExtractor<TObject> Extract<TParameter, TValue>(Expression<Func<TObject, TParameter>> expression, TValue value)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            _paramters[name] = value;

            return this;
        }

        public IParametersExtractor<TObject> Extract<TParameter, TValue>(Expression<Func<TObject, TParameter>> expression, Func<TObject, TValue> function)
        {
            return Extract(expression, function(_object));
        }

        public IParametersExtractor<TObject> ExtractBoolean<TValue>(
            string name,
            Func<TObject, bool> function,
            Func<TObject, TValue> onTrue,
            Func<TObject, TValue> onFalse)
        {
            var boolean = function(_object);

            _paramters[name] = boolean
                ? onTrue(_object)
                : onFalse(_object);

            return this;
        }

        public IParametersExtractor<TObject> ExtractBoolean<TValue>(
            Expression<Func<TObject, bool>> expression,
            Func<TObject, TValue> onTrue,
            Func<TObject, TValue> onFalse)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            var boolean = (bool) GetParameterValue(expression);

            _paramters[name] = boolean
                ? onTrue(_object)
                : onFalse(_object);

            return this;
        }

        public Dictionary<string, object> Result() => _paramters;

        private static string GetParameterName<TParameter>(Expression<Func<TObject, TParameter>> expression)
        {
            if (expression.Body is MemberExpression member)
                return member.Member.Name;

            return null;
        }

        private object GetParameterValue<TParameter>(Expression<Func<TObject, TParameter>> expression)
        {
            if (expression.Body is MemberExpression)
                return expression.Compile().Invoke(_object);

            return null;
        }
    }
}