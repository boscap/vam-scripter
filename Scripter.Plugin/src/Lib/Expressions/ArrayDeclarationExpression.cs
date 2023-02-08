﻿using System.Collections.Generic;
using System.Linq;

namespace ScripterLang
{
    public class ArrayDeclarationExpression : Expression
    {
        private readonly List<Expression> _expressions;

        public ArrayDeclarationExpression(List<Expression> expressions)
        {
            _expressions = expressions;
        }

        public override Value Evaluate(RuntimeDomain domain)
        {
            var values = new List<Value>(_expressions.Count);
            for (var i = 0; i < _expressions.Count; i++)
                values.Add(_expressions[i].Evaluate(domain));
            return Value.CreateObject(new ListReference(values));
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _expressions.Select(a => a.ToString()).ToArray())}]";
        }
    }
}
