namespace Nameless.Microservice.Web.Api.v1.Models {
    public record PostOutput {
        public string Id { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Message { get; set; } = null!;
    }
}
