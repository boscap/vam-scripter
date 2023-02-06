using ScripterLang;

namespace Scripter.Tests;

public class LanguageTests
{
    private GlobalLexicalContext _globalLexicalContext;
    private RuntimeDomain _domain;

    [SetUp]
    public void SetUp()
    {
        _globalLexicalContext = new GlobalLexicalContext();
        _domain = new RuntimeDomain();
    }

    [Test]
    public void Variables()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void Undefined()
    {
        const string source = """
            var x = undefined;
            return undefined == undefined;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("true"));
    }

    [Test]
    public void ControlFlow()
    {
        const string source = """
            var x = 1 + 1;
            var result;
            if(x == 1) {
                result = "not ok";
            } else if(x == 2) {
                result = "ok";
            }
            return result;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void ReturnExits()
    {
        const string source = """
            {
                return "ok";
            }
            throw "Did not return!";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void CustomFunctions()
    {
        const string source = """
            var x = 1;
            return MyFunction(1 * 1, "a" + "b", true == true);
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        _globalLexicalContext.Functions.Add("MyFunction", (d, args) =>
        {
            Assert.That(d.GetVariableValue("x").IntValue, Is.EqualTo(1));
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[0].ToString(), Is.EqualTo("1"));
            Assert.That(args[1].ToString(), Is.EqualTo("ab"));
            Assert.That(args[2].ToString(), Is.EqualTo("true"));
            return Value.CreateString("ok");
        });
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("ok"));
    }

    [Test]
    public void Errors()
    {
        const string source = """
            throw "Error!";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var exc = Assert.Throws<ScripterRuntimeException>(() => expression.Evaluate(_domain));
        Assert.That(exc!.Message, Is.EqualTo("Error!"));
    }

    [Test]
    public void Maths()
    {
        const string source = """
            var x = 1;
            var y = ++x;
            x = (x + y) * 2;
            x += 1;
            return x++;
            ;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("9"));
    }

    [Test]
    public void Precedence()
    {
        const string source = """
            if((1 + 1 * 2) != 3) { throw "* before +"; }
            if(!(false || true && true)) { throw "&& before ||"; }
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);

        Assert.DoesNotThrow(() => expression.Evaluate(_domain));
    }

    [Test]
    public void Strings()
    {
        const string source = """
            return "a" + 2 + true;
            ;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("a2true"));
    }

    [Test]
    public void ConditionsAndThrow()
    {
        const string source = """
            if(false) { throw "false"; }
            if(!true) { throw "!true"; }
            return "nice";
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("nice"));
    }

    [Test]
    public void Loops()
    {
        const string source = """
            var x = 0;
            while(x < 5) {
                x++;
            }
            for(var y = 0; y < 5; y++) {
                x++;
            }
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("10"));
    }

    [Test]
    public void MultipleRuns()
    {
        const string source = """
            var x = 1;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result1 = expression.Evaluate(_domain);
        var result2 = expression.Evaluate(_domain);

        Assert.That(result1.ToString(), Is.EqualTo("1"));
        Assert.That(result2.ToString(), Is.EqualTo("1"));
    }

    [Test]
    public void StaticValues()
    {
        const string source = """
            static var x = 1 + 1;
            x++;
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result1 = expression.Evaluate(_domain);
        var result2 = expression.Evaluate(_domain);

        Assert.That(result1.ToString(), Is.EqualTo("3"));
        Assert.That(result2.ToString(), Is.EqualTo("4"));
    }

    [Test]
    public void Functions()
    {
        const string source = """
            var x = 0;
            function run() {
                var x = 1;
                var y = 0;
                y += 1;
                x += y;
            }
            run();
            run();
            return x;
            """;
        var expression = Parser.Parse(source, _globalLexicalContext);
        var result = expression.Evaluate(_domain);

        Assert.That(result.ToString(), Is.EqualTo("2"));
    }
}
