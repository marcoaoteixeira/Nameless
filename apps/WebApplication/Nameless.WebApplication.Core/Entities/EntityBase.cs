using System.ComponentModel.DataAnnotations;

namespace Nameless.WebApplication.Entities {

    public abstract class EntityBase<TKey> where TKey : IEquatable<TKey> {

        #region Public Virtual Properties

        [Key]
        public virtual TKey Id { get; set; } = default!;

        #endregion
    }
}
