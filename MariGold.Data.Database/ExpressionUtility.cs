namespace MariGold.Data
{
    using System.Linq.Expressions;
    using System.Reflection;

    public static class ExpressionUtility
    {
        public static PropertyInfo GetPropertyInfo(this LambdaExpression exp)
        {
            if (exp == null)
            {
                return null;
            }

            MemberExpression ex = exp.Body as MemberExpression;

            if (ex == null)
            {
                UnaryExpression uex = exp.Body as UnaryExpression;

                if (uex != null)
                {
                    ex = uex.Operand as MemberExpression;
                }
            }

            return ex.Member as PropertyInfo;
        }
    }
}
