namespace RssServiceApi.Exceptions
{
    public class EmailVerificationException: Exception
    {
        public EmailVerificationException()
        {

        }

        public EmailVerificationException(string email)
            : base($"Email '{email}' cannot be verified")
        {

        }

        public EmailVerificationException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
