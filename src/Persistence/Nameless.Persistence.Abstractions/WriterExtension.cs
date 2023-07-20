using System.Linq.Expressions;

namespace Nameless.Persistence {
    public static class WriterExtension {
        #region Public Static Methods

        public static Task<int> SaveAsync<TEntity>(this IWriter self, SaveInstruction<TEntity> instruction, CancellationToken cancellationToken = default) where TEntity : class {
            Prevent.Against.Null(instruction, nameof(instruction));

            var instructions = new SaveInstructionCollection<TEntity> {
                instruction
            };

            return self.SaveAsync(instructions, cancellationToken);
        }

        public static Task<int> DeleteAsync<TEntity>(this IWriter self, Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : class {
            Prevent.Against.Null(filter, nameof(filter));

            var instructions = new DeleteInstructionCollection<TEntity> {
                filter
            };

            return self.DeleteAsync(instructions, cancellationToken);
        }

        #endregion
    }
}
