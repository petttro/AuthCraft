using Moq;

namespace AuthCraft.Data.Test;

public class MockStrictBehaviorTest : IDisposable
{
    protected readonly MockRepository _mockRepository;

    public MockStrictBehaviorTest()
    {
        _mockRepository = new MockRepository(MockBehavior.Strict);
    }

    public void Dispose()
    {
        _mockRepository.VerifyAll();
    }
}
