using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VivaWallet.DAL
{
    public class SoftDeleteQueryVisitor : DefaultExpressionVisitor
    {
        public const string IsDeletedColumnName = "IsDeleted";

        public override DbExpression Visit(DbScanExpression expression)
        {
            var table = (EntityType)expression.Target.ElementType;

            if (table.Properties.All(p => p.Name != IsDeletedColumnName))
            {
                return base.Visit(expression);
            }

            var binding = expression.Bind();

            return binding.Filter(
                binding
                    .VariableType
                    .Variable(binding.VariableName)
                    .Property(IsDeletedColumnName)
                    .NotEqual(DbExpression.FromBoolean(true))
            );
        }
    }
}
