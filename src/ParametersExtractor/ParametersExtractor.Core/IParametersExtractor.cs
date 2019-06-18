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

        IParametersExtractor<TObject> Extract<TParameter, TValue>(Expression<Func<TObject, TParameter>> expression, Func<TObject, TValue> function);

        Dictionary<string, object> Result();
    }
}