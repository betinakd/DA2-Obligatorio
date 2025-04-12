using Domain;
using Domain.Enums;

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
            BaseClassId = Guid.NewGuid(),
        };

        var result = simClass.GetState();

        Assert.AreEqual(SimState.Normal, result);
    }

    [TestMethod]

    public void CreateSimClass_HasAbstractState()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassAbstract",
            BaseClassId = Guid.NewGuid(),
            State = new StateAbstract()
        };

        var result = simClass.GetState();

        Assert.AreEqual(SimState.Abstract, result);
    }

    [TestMethod]

    public void CreateSimClass_HasSealedState()
    {
        var simClass = new SimClass()
        {
            Name = "TestSimClassSealed",
            BaseClassId = Guid.NewGuid(),
            State = new StateSealed()
        };

        var result = simClass.GetState();

        Assert.AreEqual(SimState.Sealed, result);
    }
}
