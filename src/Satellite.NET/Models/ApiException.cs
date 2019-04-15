using System;
using System.Collections.Generic;
using System.Net;

namespace Satellite.NET.Models
{
    /// <summary>
    /// Represents an exception throwns when an API call fails.
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// The URL of the request.
        /// </summary>
        public Uri RequestUrl { get; set; }

        /// <summary>
        /// The HTTP code returned by the server.
        /// </summary>
        public HttpStatusCode? StatusCode { get; set; }

        /// <summary>
        /// a list of errors returned by the server.
        /// </summary>
        public IEnumerable<Error> Errors { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="ApiException"/>.
        /// </summary>
        /// <param name="message">An error message.</param>
        public ApiException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// An error returned by the server after an API call.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// The title of the error.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Some detail about the returned error.
        /// </summary>
        public string Detail { get; set; }
    }
}
