using BussinesLogic;
using Domain;
using IDataAccess;
using Moq;

namespace Tests.BussinesLogic;

[TestClass]
public class ExecutionServiceTest
{
    private Mock<IExecutionDataAccess>? _mockExecuteDataAccess;
    private ExecutionService? _executionService;

    [TestInitialize]
    public void Initialize()
    {
        _mockExecuteDataAccess = new Mock<IExecutionDataAccess>(MockBehavior.Strict);
        _executionService = new ExecutionService(_mockExecuteDataAccess.Object);
    }

    [TestMethod]
    public void ExecuteMethod_BasicMethodWithoutInvocations_ReturnsFormattedOutput()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = simClass,
            Invocations = []
        };
        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("TestClass.TestMethod()");
        mockRef.Setup(r => r.GetSignatureWithClassName(signature)).Returns("TestClass.TestMethod()");
        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.AreEqual("TestClass.TestMethod() -> TestClass.TestMethod()\n", result);
    }

    [TestMethod]
    public void ExecuteMethod_MethodNotFound_ReturnsErrorMessage()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var signature = new Signature { Name = "NonExistentMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns((SimMethod)null);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.AreEqual("Error: No se encontró el método NonExistentMethod en TestClass\n", result);
    }

    [TestMethod]
    public void ExecuteMethod_NestedInvocations_FormatsProperly()
    {
        var notificadorClass = new SimClass { Id = Guid.NewGuid(), Name = "Notificador" };
        var notificacionClass = new SimClass { Id = Guid.NewGuid(), Name = "Notificacion" };
        var emailClass = new SimClass { Id = Guid.NewGuid(), Name = "Email", BaseClass = notificacionClass };

        var notificarMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "Notificar",
            RelatedClass = notificadorClass,
            RelatedClassId = notificadorClass.Id,
            Invocations = []
        };

        var enviarMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "Enviar",
            RelatedClass = notificacionClass,
            RelatedClassId = notificacionClass.Id,
            Invocations = []
        };

        var confirmarMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "Confirmar",
            RelatedClass = notificacionClass,
            RelatedClassId = notificacionClass.Id,
            Invocations = []
        };

        var enviarEmailMethod = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "Enviar",
            RelatedClass = emailClass,
            RelatedClassId = emailClass.Id,
            Invocations = []
        };

        var avisoParam = new Parameter
        {
            Id = Guid.NewGuid(),
            Name = "aviso",
            Type = notificacionClass,
            TypeId = notificacionClass.Id
        };

        var enviarSignature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "Enviar",
            Parameters = []
        };

        var confirmarSignature = new Signature
        {
            Id = Guid.NewGuid(),
            Name = "Confirmar",
            Parameters = []
        };

        var avisoRef = new ReferenceParameter { Reference = avisoParam };
        var notificadorRef = new ReferenceThis { Reference = notificadorClass };

        var otroAvisoAttr = new SimAttribute
        {
            Id = Guid.NewGuid(),
            Name = "otroAviso",
            Type = emailClass,
            TypeId = emailClass.Id,
            RelatedClass = notificadorClass,
            RelatedClassId = notificadorClass.Id
        };
        var otroAvisoRef = new ReferenceAttribute { Reference = otroAvisoAttr };

        var mockAvisoRef = new Mock<Reference>();
        mockAvisoRef.Setup(r => r.GetSimClass()).Returns(notificacionClass);
        mockAvisoRef.Setup(r => r.GetSignature(It.Is<Signature>(s => s.Name == "Enviar"))).Returns("Notificacion.Enviar()");
        mockAvisoRef.Setup(r => r.GetSignature(It.Is<Signature>(s => s.Name == "Confirmar"))).Returns("Notificacion.Confirmar()");
        mockAvisoRef.Setup(r => r.GetSignatureWithClassName(It.Is<Signature>(s => s.Name == "Enviar"))).Returns("Notificacion.Enviar()");
        mockAvisoRef.Setup(r => r.GetSignatureWithClassName(It.Is<Signature>(s => s.Name == "Confirmar"))).Returns("Notificacion.Confirmar()");

        var mockNotificadorRef = new Mock<Reference>();
        mockNotificadorRef.Setup(r => r.GetSimClass()).Returns(notificadorClass);
        mockNotificadorRef.Setup(r => r.GetSignature(It.IsAny<Signature>())).Returns("Notificador.Notificar()");
        mockNotificadorRef.Setup(r => r.GetSignatureWithClassName(It.Is<Signature>(s => s.Name == "Notificar"))).Returns("Notificador.Notificar()");

        var mockOtroAvisoRef = new Mock<Reference>();
        mockOtroAvisoRef.Setup(r => r.GetSimClass()).Returns(emailClass);
        mockOtroAvisoRef.Setup(r => r.GetSignature(It.Is<Signature>(s => s.Name == "Enviar"))).Returns("Email.Enviar()");
        mockOtroAvisoRef.Setup(r => r.GetSignature(It.Is<Signature>(s => s.Name == "Confirmar"))).Returns("Email.Confirmar()");
        mockOtroAvisoRef.Setup(r => r.GetSignatureWithClassName(It.Is<Signature>(s => s.Name == "Enviar"))).Returns("Email.Enviar()");
        mockOtroAvisoRef.Setup(r => r.GetSignatureWithClassName(It.Is<Signature>(s => s.Name == "Confirmar"))).Returns("Email.Confirmar()");

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(It.IsAny<SimClass>(), It.Is<Signature>(s => s.Name == "Notificar")))
            .Returns(notificarMethod);

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(It.IsAny<SimClass>(), It.Is<Signature>(s => s.Name == "Enviar")))
            .Returns((SimClass c, Signature s) => c.Name == "Email" ? enviarEmailMethod : enviarMethod);

        _mockExecuteDataAccess
            .Setup(m => m.FindMethodInHierarchy(It.IsAny<SimClass>(), It.Is<Signature>(s => s.Name == "Confirmar")))
            .Returns(confirmarMethod);

        notificarMethod.Invocations =
        [
            new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = mockAvisoRef.Object,
            Signature = enviarSignature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = mockAvisoRef.Object,
            Signature = confirmarSignature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = mockOtroAvisoRef.Object,
            Signature = enviarSignature
        },
        new Invocation
        {
            Id = Guid.NewGuid(),
            Reference = mockOtroAvisoRef.Object,
            Signature = confirmarSignature
        },
    ];

        var notificarSignature = new Signature { Id = Guid.NewGuid(), Name = "Notificar" };
        var result = _executionService.ExecuteMethod(mockNotificadorRef.Object, mockNotificadorRef.Object, notificarSignature);

        var expectedOutput = "Notificador.Notificar() -> Notificador.Notificar()\n" +
                            "    Notificacion.Enviar() -> Notificacion.Enviar()\n" +
                            "    Notificacion.Confirmar() -> Notificacion.Confirmar()\n" +
                            "    Email.Enviar() -> Email.Enviar()\n" +
                            "    Email.Confirmar() -> Notificacion.Confirmar()\n";

        Assert.AreEqual(expectedOutput, result);
    }

    [TestMethod]
    public void ExecuteMethod_RecursiveCall_DetectsRecursion()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "Recursive" };
        var methodId = Guid.NewGuid();
        var signature = new Signature { Name = "RecursiveMethod", Parameters = [] };

        var method = new SimMethod
        {
            Id = methodId,
            Name = "RecursiveMethod",
            RelatedClass = simClass,
            Invocations = []
        };

        var recursiveInvocation = new Invocation
        {
            Reference = new ReferenceThis { Reference = simClass },
            Signature = signature
        };
        method.Invocations.Add(recursiveInvocation);

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignature(signature)).Returns("Recursive.RecursiveMethod()");

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.IsTrue(result.Contains("/* recursión */"));
    }

    [TestMethod]
    public void ExecuteMethod_LevelZero_UsesGetSignatureWithClassName()
    {
        var simClass = new SimClass { Id = Guid.NewGuid(), Name = "TestClass" };
        var method = new SimMethod
        {
            Id = Guid.NewGuid(),
            Name = "TestMethod",
            RelatedClass = simClass,
            Invocations = []
        };
        var signature = new Signature { Name = "TestMethod", Parameters = [] };

        var mockRef = new Mock<Reference>();
        mockRef.Setup(r => r.GetSimClass()).Returns(simClass);
        mockRef.Setup(r => r.GetSignatureWithClassName(signature)).Returns("TestClass.TestMethod()");
        mockRef.Setup(r => r.GetSignature(signature)).Returns("TestClass.TestMethod()");

        _mockExecuteDataAccess!
            .Setup(m => m.FindMethodInHierarchy(simClass, signature))
            .Returns(method);

        var result = _executionService!.ExecuteMethod(mockRef.Object, mockRef.Object, signature);

        Assert.IsTrue(result.StartsWith("TestClass.TestMethod() -> TestClass.TestMethod()"));
    }
}
