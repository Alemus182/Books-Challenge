namespace Api.Components
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public static class Auth
        { 
            public const string Login = Root + "/auth/Login";
        }

        public static class StoriesRoutes
        {
            public const string Group = Root + "/Stories";
            public const string GetNewestStories = $"/Get-Newest/{{page:int}}";
            public const string FindStoriesByFilters = "/Find-Stories-by-Filter/";
        }

        public static class BooksRoutes
        {
            public const string Group = Root + "/Books";
            public const string SearchBooks = "/Search";
        }
    }
}
