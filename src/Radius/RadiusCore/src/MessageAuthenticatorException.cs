using System;

namespace RadiusCore
{
    public class MessageAuthenticatorException(string message) : InvalidOperationException(message);
}