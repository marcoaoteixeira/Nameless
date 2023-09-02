using AutoMapper;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;
using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Requests;

namespace Nameless.Checklist.Web.Domain.Mappings {
    public sealed class ChecklistItemMapping : Profile {
        #region Public Constructors

        public ChecklistItemMapping() {
            CreateMap<CreateChecklistItemInput, CreateChecklistItemRequest>();
            CreateMap<CreateChecklistItemRequest, ChecklistItem>();
            CreateMap<ChecklistItem, ChecklistItemDto>();
            CreateMap<ChecklistItemDto, ChecklistItemOutput>();

            CreateMap<ListChecklistItemsInput, ListChecklistItemsRequest>();

            CreateMap<UpdateChecklistItemInput, UpdateChecklistItemRequest>();
        }

        #endregion
    }
}
