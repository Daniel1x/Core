namespace DL
{
    public static class CoreData
    {
        public const string MY_PREFIX = "DL ";
        public const string ROOT = MY_PREFIX + "Editor/";
        public const string CREATION_TAB_NAME = MY_PREFIX + "Tools/";
        public const string WINDOW_TAB_NAME = MY_PREFIX + "Windows/";
        public const string SCRIPTABLE_CREATION_TAB_NAME = CREATION_TAB_NAME + "Scriptables/";

        public const bool IS_EDITOR =
#if UNITY_EDITOR
            true;
#else
            false;
#endif

        public const bool IS_BUILD = !IS_EDITOR;

        public const bool IS_DEVELOPMENT_BUILD =
#if DEVELOPMENT_BUILD
            true;
#else
            false;
#endif

        public const bool IS_SHIPMENT_BUILD = IS_BUILD && IS_DEVELOPMENT_BUILD == false;
    }
}
