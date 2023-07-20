using System.Collections;
using System.Linq.Expressions;

namespace Nameless.Persistence {
    public sealed class DeleteInstruction<TEntity> where TEntity : class {
        #region Public Properties

        public Expression<Func<TEntity, bool>> Filter { get; }

        #endregion

        #region Private Constructors

        private DeleteInstruction(Expression<Func<TEntity, bool>> filter) {
            Filter = Prevent.Against.Null(filter, nameof(filter));
        }

        #endregion

        #region Public Static Methods

        public static DeleteInstruction<TEntity> Create(Expression<Func<TEntity, bool>> filter)
            => new(filter);

        #endregion
    }

    public sealed class DeleteInstructionCollection<TEntity> : IEnumerable<DeleteInstruction<TEntity>> where TEntity : class {
        #region Private Read-Only Fields

        private readonly List<DeleteInstruction<TEntity>> _instructions = new();

        #endregion

        #region Public Methods

        public DeleteInstructionCollection<TEntity> Add(Expression<Func<TEntity, bool>> filter) {
            _instructions.Add(DeleteInstruction<TEntity>.Create(filter));
            return this;
        }

        public void Clear() => _instructions.Clear();

        #endregion

        #region IEnumerable<DeleteInstruction<TEntity>> Members

        public IEnumerator<DeleteInstruction<TEntity>> GetEnumerator()
            => _instructions.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _instructions.GetEnumerator();

        #endregion
    }
}
