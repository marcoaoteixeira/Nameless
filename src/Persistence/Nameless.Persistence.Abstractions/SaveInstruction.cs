using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Nameless.Persistence {
    public sealed class SaveInstruction<TEntity> where TEntity : class {

        #region Public Properties

        public TEntity Entity { get; private set; }
        public Expression<Func<TEntity, bool>>? Filter { get; private set; }
        public Expression<Func<TEntity, TEntity>>? Patch { get; private set; }
        public SaveMode Mode { get; private set; }

        #endregion

        #region Private Constructors

        private SaveInstruction() {
            Entity = default!;
        }

        #endregion

        #region Public Static Methods

        public static SaveInstruction<TEntity> Insert(TEntity entity) {
            Garda.Prevent.Null(entity, nameof(entity));

            return new SaveInstruction<TEntity> {
                Entity = entity,
                Filter = null,
                Patch = null,
                Mode = SaveMode.Insert
            };
        }

        public static SaveInstruction<TEntity> Update(TEntity entity) {
            Garda.Prevent.Null(entity, nameof(entity));

            return new SaveInstruction<TEntity> {
                Entity = entity,
                Filter = null,
                Patch = null,
                Mode = SaveMode.Update
            };
        }

        public static SaveInstruction<TEntity> Update(TEntity entity, Expression<Func<TEntity, bool>> filter) {
            Garda.Prevent.Null(entity, nameof(entity));
            Garda.Prevent.Null(filter, nameof(filter));

            return new SaveInstruction<TEntity> {
                Entity = entity,
                Filter = filter,
                Patch = null,
                Mode = SaveMode.Update
            };
        }

        public static SaveInstruction<TEntity> Update(Expression<Func<TEntity, TEntity>> patch, Expression<Func<TEntity, bool>> filter) {
            Garda.Prevent.Null(patch, nameof(patch));
            Garda.Prevent.Null(filter, nameof(filter));

            return new SaveInstruction<TEntity> {
                Entity = null!,
                Filter = filter,
                Patch = patch,
                Mode = SaveMode.Update
            };
        }

        public static SaveInstruction<TEntity> UpSert(TEntity entity) {
            Garda.Prevent.Null(entity, nameof(entity));

            return new SaveInstruction<TEntity> {
                Entity = entity,
                Filter = null,
                Patch = null,
                Mode = SaveMode.UpSert
            };
        }

        public static SaveInstruction<TEntity> UpSert(TEntity entity, Expression<Func<TEntity, bool>> filter) {
            Garda.Prevent.Null(entity, nameof(entity));
            Garda.Prevent.Null(filter, nameof(filter));

            return new SaveInstruction<TEntity> {
                Entity = entity,
                Filter = filter,
                Patch = null,
                Mode = SaveMode.UpSert
            };
        }

        #endregion
    }

    public enum SaveMode {
        Insert = 0,

        Update = 1,

        UpSert = 2
    }

    public sealed class SaveInstructionCollection<TEntity> : Collection<SaveInstruction<TEntity>> where TEntity : class {
        #region Public Methods

        public SaveInstructionCollection<TEntity> AddInsert(TEntity entity) {
            Add(SaveInstruction<TEntity>.Insert(entity));
            return this;
        }

        public SaveInstructionCollection<TEntity> AddUpdate(TEntity entity) {
            Add(SaveInstruction<TEntity>.Update(entity));
            return this;
        }

        public SaveInstructionCollection<TEntity> AddUpdate(TEntity entity, Expression<Func<TEntity, bool>> filter) {
            Add(SaveInstruction<TEntity>.Update(entity, filter));
            return this;
        }

        public SaveInstructionCollection<TEntity> AddUpdate(Expression<Func<TEntity, TEntity>> patch, Expression<Func<TEntity, bool>> filter) {
            Add(SaveInstruction<TEntity>.Update(patch, filter));
            return this;
        }

        public SaveInstructionCollection<TEntity> AddUpSert(TEntity entity) {
            Add(SaveInstruction<TEntity>.UpSert(entity));
            return this;
        }

        public SaveInstructionCollection<TEntity> AddUpSert(TEntity entity, Expression<Func<TEntity, bool>> filter) {
            Add(SaveInstruction<TEntity>.UpSert(entity, filter));
            return this;
        }

        #endregion
    }
}
