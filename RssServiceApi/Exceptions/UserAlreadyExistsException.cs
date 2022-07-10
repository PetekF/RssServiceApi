namespace RssServiceApi.Exceptions
{
    public class UserAlreadyExistsException: Exception
    {
        public UserAlreadyExistsException()
        {

        }

        public UserAlreadyExistsException(string email)
            :base($"User '{email}' already exists")
        {

        }

        public UserAlreadyExistsException(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
}
