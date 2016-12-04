namespace database
{
    /// <summary>
    /// An enum-like class with readable strings that can be passed as const values.
    /// </summary>
    public class SentenceType
    {
        public const string delimiter = "_";

        public const string NeverMindKeepGoing = "Never mind, let's keep going.";
        public const string ItsJustBlank = "It's just _ ."; // Useful for adjectives.
        public const string ItsSimilarToBlank = "It's similar to _ .";
        public const string RememberBlank = "Remember _ ?"; // Doesn't affect understanding or confusion much.
        public const string ItsBlank = "It's _ .";
        public const string ItsLikeBlankButBlank = "It's like _ , but _ .";
        public const string ItsBlankWithBlank = "It's _ with _ .";
        public const string ItsABlankFromBlank = "It's a _ from _ .";
        public const string ThinkOfBlankExceptBlank = "Think of _ , except with _ .";
        public const string ItsThreeBlanks = "It's _ with _ and _ .";
        public const string IfYouMixTheseThree = "If you mixed _ , _ and _ you'd get that.";

        public static string []sentanceArray = {NeverMindKeepGoing,
                                                ItsJustBlank,
                                                ItsSimilarToBlank,
                                                RememberBlank,
                                                ItsBlank,
                                                ItsLikeBlankButBlank,
                                                ItsBlankWithBlank,
                                                ItsABlankFromBlank,
                                                ThinkOfBlankExceptBlank,
                                                ItsThreeBlanks,
                                                IfYouMixTheseThree };
    }
}