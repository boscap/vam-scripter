﻿using System;
using ScripterLang;

public class DateTimeReference : ObjectReference
{
    private readonly DateTime _value;

    public DateTimeReference(DateTime value)
    {
        _value = value;
    }

    public override Value Get(string name)
    {
        switch (name)
        {
            case "toString":
                return fn(ToString);
            default:
                return base.Get(name);
        }
    }

    private Value ToString(LexicalContext context, Value[] args)
    {
        return _value.ToString(args.Length == 1 ? args[0].AsString : "s");
    }
}
