using Domain;

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

        Assert.AreEqual("Normal", result);
    }
}
