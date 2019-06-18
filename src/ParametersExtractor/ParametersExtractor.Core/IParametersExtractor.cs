using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ParametersExtractor.Core
{
    public interface IParametersExtractor<TObject> where TObject : class
    {
        IParametersExtractor<TObject> Extract<TParameter>(string name, Func<TObject, TParameter> function);

        IParametersExtractor<TObject> Extract<TParameter>(Expression<Func<TObject, TParameter>> expression);

        IParametersExtractor<TObject> Extract<TParameter, TValue>(Expression<Func<TObject, TParameter>> expression, TValue value);

        IParametersExtractor<TObject> Extract<TParameter, TValue>(
            Expression<Func<TObject, TParameter>> expression,
            Func<TObject, TValue> function);

        IParametersExtractor<TObject> ExtractBoolean<TValue>(
            string name,
            Func<TObject, bool> function,
            Func<TObject, TValue> onTrue = null,
            Func<TObject, TValue> onFalse = null);

        IParametersExtractor<TObject> ExtractBoolean<TValue>(
            Expression<Func<TObject, bool>> expression,
            Func<TObject, TValue> onTrue = null,
            Func<TObject, TValue> onFalse = null);

        Dictionary<string, object> Result();
    }
}