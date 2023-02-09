﻿using System.Collections.Generic;

namespace ScripterLang
{
    public class ImportExpression : Expression
    {
        private readonly List<string> _imports;
        private readonly string _module;
        private readonly LexicalContext _context;
        private readonly GlobalLexicalContext _globalContext;

        public ImportExpression(List<string> imports, string module, LexicalContext context)
        {
            _imports = imports;
            _module = module;
            _context = context;
            _globalContext = _context.GetGlobalContext();
        }

        public override Value Evaluate()
        {
            var module = _globalContext.GetModule(_module);
            var exports = module.Import();
            foreach (var import in _imports)
            {
                Value value;
                if (!exports.TryGetValue(import, out value))
                    throw new ScripterRuntimeException($"Module '{module.ModuleName}' does not export '{import}'");
                _context.Variables[import] = value;
            }
            return Value.Void;
        }

        public override string ToString()
        {
            return $"import {{ {string.Join(", ", _imports)} }} from \"{_module}\"";
        }
    }
}