using Domain;
using Domain.Enums;
using Domain.Exceptions;

namespace Tests.Domain;

[TestClass]
public class SimClassTest
{
    [TestMethod]
    public void CreateSimClass_HasNormalState()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClass",
        };
        simClass.SetState(new StateNormal());
        var result = simClass.GetState();

        Assert.AreEqual(SimState.Normal, result);
    }

    [TestMethod]

    public void CreateSimClass_HasAbstractState()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassAbstract",
        };

        simClass.SetState(new StateAbstract());
        var result = simClass.GetState();

        Assert.AreEqual(SimState.Abstract, result);
    }

    [TestMethod]

    public void CreateSimClass_HasSealedState()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassSealed",
        };

        simClass.SetState(new StateSealed());
        var result = simClass.GetState();

        Assert.AreEqual(SimState.Sealed, result);
    }

    [TestMethod]
    public void CreateSimClass_HasBaseClass()
    {
        var baseClass = new SimClass()
        {
            Name = "BaseClass",
        };
        baseClass.SetState(new StateNormal());

        var simClass = new SimClass()
        {
            Name = "DerivedClass",
            BaseClass = baseClass
        };

        Assert.IsNotNull(simClass.BaseClass);
        Assert.AreEqual("BaseClass", simClass.BaseClass?.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void SetBaseClass_ThrowsException_WhenBaseClassIsSealed()
    {
        var baseClass = new SimClass
        {
            Name = "SealedBaseClass"
        };
        baseClass.SetState(new StateSealed());

        var simClass = new SimClass
        {
            Name = "DerivedClass"
        };

        simClass.BaseClass = baseClass;
    }

    [TestMethod]
    public void AddCorrectAttributeToSimClass_CorrectlyAddsIt()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Accesibility = SimAccesibility.Public };

        simClass.AddAttribute(simAttribute);

        Assert.IsTrue(simClass.Attributes.Any(a => a.Name == "TestAttribute"));
    }

    [TestMethod]
    [ExpectedException(typeof(SimClassInvalidAttribute))]
    public void AddAttributeWithRepetedNameClass_ShouldThrowSimClassInvalidAttribute()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassWithAttribute"
        };
        var relatedClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var typeClass = new SimClass()
        {
            Name = "TypeClass"
        };
        var simAttribute = new SimAttribute() { Name = "TestAttribute", RelatedClass = relatedClass, Type = typeClass, Accesibility = SimAccesibility.Public };
        var repeatedName = new SimAttribute() { Name = "TestAttribute", RelatedClass = typeClass, Type = relatedClass, Accesibility = SimAccesibility.Private };

        simClass.AddAttribute(simAttribute);
        simClass.AddAttribute(repeatedName);
    }
}
