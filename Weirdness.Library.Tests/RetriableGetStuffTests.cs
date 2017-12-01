namespace Weirdness.Library.Tests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class RetriableGetStuffTests
    {
        [Test]
        public async Task DoNotRetrySuccessfulAttempts()
        {
            // Arramge
            var id = Guid.NewGuid();
            var expected = "{ }";

            var repository = new Mock<IDocumentRepository>();
            repository.Setup(r => r.Get<string>(id)).ReturnsAsync(expected);

            var options = new RetryOptions(5);
            var stuffGetter = new RetriableGetStuff(repository.Object, options);

            // Act
            var actual = await stuffGetter.Get<string>(id);

            // Assert
            actual.Should().Be(expected);
            repository.Verify(r => r.Get<string>(id), Times.Once);
        }

        [TestCase(429, TestName = "DocumentClientExceptionWithHttpStatusCodeOf429ShouldBeRetried")]
        public async Task RetriableDocumentClientExceptionsShouldBeRetried(int httpStatusCode)
        {
            // Arrange
            var id = Guid.NewGuid();
            var expected = "{ }";

            var repository = new Mock<IDocumentRepository>();
            repository.SetupSequence(r => r.Get<string>(id))
                .ThrowsAsync(DocumentClientExceptionFactory.Create((HttpStatusCode)httpStatusCode))
                .ReturnsAsync(expected);

            var options = new RetryOptions(5);
            var stuffGetter = new RetriableGetStuff(repository.Object, options);

            // Act
            var actual = await stuffGetter.Get<string>(id);

            // Assert
            actual.Should().Be(expected);
            repository.Verify(r => r.Get<string>(id), Times.Exactly(2));
        }

        [TestCase(429, 3, TestName = "DocumentClientExceptionWithHttpStatusCodeOf429ShouldBeRetriedThreeTimes")]
        public void RetriableDocumentClientExceptionsShouldBeRetriedALimitedNumberOfTimes(int httpStatusCode, int maxRetries)
        {
            // Arrange
            var id = Guid.NewGuid();

            var repository = new Mock<IDocumentRepository>();
            repository.Setup(r => r.Get<string>(id)).ThrowsAsync(DocumentClientExceptionFactory.Create((HttpStatusCode)httpStatusCode));

            var options = new RetryOptions(maxRetries);
            var stuffGetter = new RetriableGetStuff(repository.Object, options);

            // Act
            AsyncTestDelegate actor = async () => await stuffGetter.Get<string>(id);

            // Assert
            Assert.ThrowsAsync<DocumentClientException>(actor).StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
            repository.Verify(r => r.Get<string>(id), Times.Exactly(maxRetries));
        }
    }
}
