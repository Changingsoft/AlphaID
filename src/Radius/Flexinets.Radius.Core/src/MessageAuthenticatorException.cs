using System;

namespace Flexinets.Radius.Core
{
    public class MessageAuthenticatorException(string message) : InvalidOperationException(message);
}