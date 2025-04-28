using Domain;

namespace Tests.Domain;

[TestClass]
public class SimMethodTest
{
    [TestMethod]
    public void MatchSignature_WithMatchingSignature_ReturnsTrue()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id },
                new ParameterSignature { Name = "y", TypeId = stringType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentName_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType }
            ]
        };

        var signature = new Signature
        {
            Name = "DifferentMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterCount_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterTypes_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };
        var doubleType = new SimClass { Id = Guid.NewGuid(), Name = "double" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = intType.Id },
                new ParameterSignature { Name = "y", TypeId = doubleType.Id } // Different type
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void MatchSignature_WithDifferentParameterOrder_ReturnsFalse()
    {
        var intType = new SimClass { Id = Guid.NewGuid(), Name = "int" };
        var stringType = new SimClass { Id = Guid.NewGuid(), Name = "string" };

        var method = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = intType.Id, Type = intType },
                new Parameter { Name = "param2", TypeId = stringType.Id, Type = stringType }
            ]
        };

        var signature = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "y", TypeId = stringType.Id }, // Swapped order
                new ParameterSignature { Name = "x", TypeId = intType.Id }
            ]
        };

        var result = method.MatchSignature(signature);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void GetMethodSignature_ReturnsCorrectFormat()
    {
        var method = new SimMethod
        {
            Name = "MyMethod"
        };

        var signature = new Signature
        {
            Name = "MyMethod",
            Parameters =
            [
                new ParameterSignature { Name = "a" },
                new ParameterSignature { Name = "b" }
            ]
        };

        var result = method.GetMethodSignature(signature);

        Assert.AreEqual("MyMethod.MyMethod(a, b)", result);
    }
}
