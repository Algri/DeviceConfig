namespace IoT.RPiController.Data.Constants;

public static class TokenExpirationTime
{
        public const int MinTokenExpirationTime = 0;
        public const int MaxTokenExpirationTime = 5258880;
        public const int DefaultAccessTokenExpirationTime = 480;
        public const int DefaultRefreshTokenExpirationTime = 10080;
}