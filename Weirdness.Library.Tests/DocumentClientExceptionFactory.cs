namespace Weirdness.Library.Tests
{
    using System.Net;
    using System.Net.Http.Headers;
    using System.Reflection;
    using Microsoft.Azure.Documents;

    public static class DocumentClientExceptionFactory
    {
        /// <summary>
        /// Uses reflection to create an instance of the DocumentClientException class.
        /// </summary>
        /// <param name="httpStatusCode">The HttpStatusCode the created exception should report.</param>
        /// <returns>An instance of the DocumentClientException class reporting the specified HttpStatusCode.</returns>
        /// <remarks>
        /// Sourced from https://stackoverflow.com/questions/35618723/how-do-i-mock-the-documentclientexception-that-the-azure-documentdb-client-libra
        /// </remarks>
        public static DocumentClientException Create(HttpStatusCode httpStatusCode)
        {
            var type = typeof(DocumentClientException);

            // Using the overload with 3 parameters (error, responseheaders, statuscode)
            var documentClientExceptionInstance = type.Assembly.CreateInstance(type.FullName,
                false, BindingFlags.Instance | BindingFlags.NonPublic, null,
                new object[] { new Error(), (HttpResponseHeaders)null, httpStatusCode }, null, null);

            return (DocumentClientException)documentClientExceptionInstance;
        }
    }
}
