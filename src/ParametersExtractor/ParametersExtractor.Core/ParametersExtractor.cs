using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ParametersExtractor.Core
{
    public sealed class ParametersExtractor<TObject> where TObject : class
    {
        private readonly TObject _object;

        private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();

        public ParametersExtractor(TObject @object)
        {
            _object = @object;
        }

        /// <summary>
        /// Извлекает значение (из выражения) и имя свойства, добавляет параметр с извлечённым именем и значением.
        /// </summary>
        public ParametersExtractor<TObject> Extract<TParameter>(Expression<Func<TObject, TParameter>> expression)
        {
            var value = GetParameterValue(expression);

            return value != null
                ? ExtractAsValue(expression, value)
                : this;
        }

        /// <summary>
        /// Извлекает значение свойства, добавляет параметр с указанным именем и извлечённым значением.
        /// </summary>
        public ParametersExtractor<TObject> ExtractAs<TParameter>(string name, Func<TObject, TParameter> function)
        {
            _parameters[name] = function(_object);

            return this;
        }

        /// <summary>
        /// Извлекает имя свойства, добавляет параметр с извлечённым именем и пустым значением (NULL).
        /// </summary>
        public ParametersExtractor<TObject> ExtractAsEmpty<TParameter>(Expression<Func<TObject, TParameter>> expression)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            _parameters[name] = null;

            return this;
        }

        /// <summary>
        /// Извлекает имя свойства, добавляет параметр с извлечённым именем и указанным значением.
        /// </summary>
        public ParametersExtractor<TObject> ExtractAsValue<TParameter, TValue>(Expression<Func<TObject, TParameter>> expression, TValue value)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            _parameters[name] = value;

            return this;
        }

        /// <summary>
        /// Извлекает значение (из функции) и имя свойства, добавляет параметр с извлечённым именем и значением.
        /// </summary>
        public ParametersExtractor<TObject> Extract<TParameter, TValue>(
            Expression<Func<TObject, TParameter>> expression,
            Func<TObject, TValue> function)
        {
            return ExtractAsValue(expression, function(_object));
        }

        /// <summary>
        /// Извлекает значение (из функции) и имя свойства, добавляет параметр с извлечённым именем и значением.
        /// </summary>
        public ParametersExtractor<TObject> Extract<TValue>(
            Expression<Func<TObject, bool>> expression,
            Func<TObject, TValue> onTrue = null,
            Func<TObject, TValue> onFalse = null)
        {
            var name = GetParameterName(expression);

            if (string.IsNullOrWhiteSpace(name))
                return this;

            var value = (bool) GetParameterValue(expression);

            return ExtractAs(name, _ => value, onTrue, onFalse);
        }

        /// <summary>
        /// Извлекает значение (из функции), добавляет параметр с извлечённым значением и указанным именем.
        /// </summary>
        public ParametersExtractor<TObject> ExtractAs<TValue>(
            string name,
            Func<TObject, bool> function,
            Func<TObject, TValue> onTrue = null,
            Func<TObject, TValue> onFalse = null)
        {
            var value = function(_object);

            switch (value)
            {
                case true when onTrue != null:
                    _parameters[name] = onTrue(_object);
                    return this;

                case false when onFalse != null:
                    _parameters[name] = onFalse(_object);
                    return this;
            }

            _parameters[name] = value;

            return this;
        }

        public Dictionary<string, object> Result() => _parameters;

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