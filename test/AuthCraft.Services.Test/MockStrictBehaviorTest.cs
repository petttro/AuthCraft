using System;
using Moq;

namespace AuthCraft.Services.Tests;

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
