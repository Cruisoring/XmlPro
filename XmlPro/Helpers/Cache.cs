using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace XmlPro.Helpers
{
    public class Cache<T> where T : class
    {
        private T value = null;
        private readonly Func<T> getter;

        public T Value => value ??= getter();

        public Cache([NotNull] Func<T> getFunc)
        {
            this.getter = getFunc;
        }
    }
}
