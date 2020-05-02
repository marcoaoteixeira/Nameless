namespace Nameless.ObjectMapper {
    public static class MapperExtension {
        #region Public Static Methods

        public static TTo Map<TTo, TFrom> (this IMapper source, TFrom instance) {
            if (source == null) { return default; }
            if (instance == null) { return default; }

            return (TTo) source.Map (typeof (TFrom), typeof (TTo), instance);
        }

        #endregion
    }
}