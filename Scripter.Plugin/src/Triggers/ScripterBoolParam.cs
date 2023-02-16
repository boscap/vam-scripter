﻿using System.Globalization;
using ScripterLang;
using SimpleJSON;

public class ScripterBoolParam : ScripterParamBase
{
    public const string Type = "BoolParam";

    private readonly JSONStorableBool _valueJSON;

    public ScripterBoolParam(string name, bool startingValue)
    {
        var scripter = Scripter.Singleton;
        var existing = scripter.GetBoolJSONParam(name);
        if (existing == null)
        {
            _valueJSON = new JSONStorableBool(name, startingValue);
            scripter.RegisterBool(_valueJSON);
        }
        else
        {
            _valueJSON = existing;
            _valueJSON.defaultVal = startingValue;
        }
    }

    public static ScripterParamBase FromJSONImpl(JSONNode json)
    {
        var trigger = new ScripterBoolParam(
            json["Name"],
            json["StartingValue"].AsBool
        );
        trigger._valueJSON.val = json["Val"].AsBool;
        return trigger;
    }

    public override JSONClass GetJSON()
    {
        var json = new JSONClass
        {
            { "Type", Type },
            { "Name", _valueJSON.name },
            { "StartingValue", _valueJSON.defaultVal.ToString(CultureInfo.InvariantCulture) },
            { "Val", _valueJSON.val.ToString(CultureInfo.InvariantCulture) },
        };
        return json;
    }

    public override Value GetProperty(string name)
    {
        switch (name)
        {
            case "val":
                return _valueJSON.val;
            case "onChange":
                return Func(OnChange);
            default:
                return base.GetProperty(name);
        }
    }

    public override void SetProperty(string name, Value value)
    {
        switch (name)
        {
            case "val":
                _valueJSON.valNoCallback = value.AsBool;
                break;
            default:
                base.SetProperty(name, value);
                break;
        }
    }

    private readonly Value[] _callbackArgs = new Value[1];
    private Value OnChange(LexicalContext context, Value[] args)
    {
        ValidateArgumentsLength(nameof(OnChange), args, 1);
        var fn = args[0].AsFunction;
        _valueJSON.setCallbackFunction = val =>
        {
            _callbackArgs[0] = val;
            fn(context, _callbackArgs);
        };
        return Value.Void;
    }
}
