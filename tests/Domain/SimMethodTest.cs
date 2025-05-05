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
            RelatedClass = new SimClass { Id = Guid.NewGuid(), Name = "MyClass" },
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

        Assert.AreEqual("MyClass.MyMethod(a, b)", result);
    }

    [TestMethod]
    public void Equals_WithNonSimMethodObject_ReturnsFalse()
    {
        var method = new SimMethod { Name = "TestMethod" };
        var nonSimMethodObject = new object();

        var result = method.Equals(nonSimMethodObject);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentMethodName_ReturnsFalse()
    {
        var method1 = new SimMethod { Name = "TestMethod" };
        var method2 = new SimMethod { Name = "DifferentMethod" };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterCount_ReturnsFalse()
    {
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId },
                new Parameter { Name = "param2", TypeId = typeId }
            ]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterNames_ReturnsTrue()
    {
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "differentParam", TypeId = typeId }]
        };

        var result = method1.Equals(method2);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Equals_WithDifferentParameterTypes_ReturnsFalse()
    {
        var typeId1 = Guid.NewGuid();
        var typeId2 = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId1 }]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [new Parameter { Name = "param1", TypeId = typeId2 }]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_TypeIdComparisonFalseCase()
    {
        var typeId1 = Guid.NewGuid();
        var typeId2 = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId1 }
            ]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId2 }
            ]
        };

        var result = method1.Equals(method2);

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Equals_WithNullTypeIds_ShouldHandleCorrectly()
    {
        var typeId = Guid.NewGuid();

        var method1 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = null }
            ]
        };

        var method2 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "differentName", TypeId = null } // También null
            ]
        };

        var method3 = new SimMethod
        {
            Name = "TestMethod",
            Parameters = [
                new Parameter { Name = "param1", TypeId = typeId } // No es null
            ]
        };

        var resultBothNull = method1.Equals(method2);
        var resultOneNull = method1.Equals(method3);

        Assert.IsTrue(resultBothNull);
        Assert.IsFalse(resultOneNull);
    }

    [TestMethod]
    public void MatchSignature_WithNullTypeIds_HandlesCorrectly()
    {
        var typeId = Guid.NewGuid();

        var methodBothNull = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = null }
            ]
        };

        var signatureBothNull = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = null }
            ]
        };

        var signatureNotNull = new Signature
        {
            Name = "TestMethod",
            Parameters =
            [
                new ParameterSignature { Name = "x", TypeId = typeId }
            ]
        };

        var methodNotNull = new SimMethod
        {
            Name = "TestMethod",
            Parameters =
            [
                new Parameter { Name = "param1", TypeId = typeId }
            ]
        };

        var resultBothNull = methodBothNull.MatchSignature(signatureBothNull);
        var resultMethodNullSignatureNotNull = methodBothNull.MatchSignature(signatureNotNull);
        var resultMethodNotNullSignatureNull = methodNotNull.MatchSignature(signatureBothNull);

        Assert.IsTrue(resultBothNull, "Cuando ambos TypeId son null, deberían considerarse iguales");
        Assert.IsFalse(resultMethodNullSignatureNotNull, "Cuando un TypeId es null y el otro no, deberían considerarse diferentes");
        Assert.IsFalse(resultMethodNotNullSignatureNull, "Cuando un TypeId es null y el otro no, deberían considerarse diferentes");
    }

    [TestMethod]
    [ExpectedException(typeof(NotImplementedException))]
    public void GetHashCode_ThrowsNotImplementedException()
    {
        var method = new SimMethod { Name = "TestMethod" };

        method.GetHashCode();
    }
}
