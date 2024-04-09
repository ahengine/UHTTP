using System.Collections.Generic;

namespace UHTTP
{
    public enum HTTPRequestMethod { GET,POST,PUT,HEAD,CREATE,DELETE }

    public enum HTTPResponseCodes
    {
        OK_200 = 200,
        AUTHORIZED_201 = 201,
        NOT_FOUND_404 = 404,
        SERVER_ERROR_500 = 500,
        UNAUTHORIZED_401 = 401,
        FORBIDEN_403 = 401,
    }   

    public static class HTTPHeaderHelper
    {
        public static KeyValuePair<string, string> ContentType = 
            new KeyValuePair<string, string>("Content-Type", "application/json");

        public static KeyValuePair<string, string> Accept = 
            new KeyValuePair<string, string>("Accept", "application/json");
    }
}