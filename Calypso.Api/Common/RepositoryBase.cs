using System;
using Azure.Core;

namespace Calypso.Api.Common
{
    public abstract class RepositoryBase
    {
        protected TimeSpan Delay => TimeSpan.FromSeconds(2);
        protected int MaxRetries => 5;
        protected RetryMode Mode => RetryMode.Exponential;
        protected TimeSpan MaxDelay => TimeSpan.FromSeconds(10);
    }
}
