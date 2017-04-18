namespace MariGold.Data
{
    public static class Config
    {
        private static bool underscoreToPascalCase;

        public static bool UnderscoreToPascalCase
        {
            get
            {
                return underscoreToPascalCase;
            }
            set
            {
                Db.GetConverter().ClearAllTypes();

                underscoreToPascalCase = value;
            }
        }
    }
}
