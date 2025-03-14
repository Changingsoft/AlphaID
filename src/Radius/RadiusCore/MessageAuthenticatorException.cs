namespace RadiusCore;

/// <summary>
/// 
/// </summary>
/// <param name="message"></param>
public class MessageAuthenticatorException(string message) : InvalidOperationException(message);