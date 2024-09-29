using AutoMapper;
using Nameless.Checklist.Web.Api.v1.Models.Input;
using Nameless.Checklist.Web.Api.v1.Models.Output;

using Nameless.Checklist.Web.Domain.Dtos;
using Nameless.Checklist.Web.Domain.Entities;
using Nameless.Checklist.Web.Domain.Requests;

using v2ChecklistItemOutput = Nameless.Checklist.Web.Api.v2.Models.Output.ChecklistItemOutput;
using v2ListChecklistItemsInput = Nameless.Checklist.Web.Api.v2.Models.Input.ListChecklistItemsInput;

namespace Nameless.Checklist.Web.Domain.Mappings;

public sealed class ChecklistItemMapping : Profile {
    public ChecklistItemMapping() {
        CreateMap<CreateChecklistItemInput, CreateChecklistItemRequest>();
        CreateMap<CreateChecklistItemRequest, ChecklistItem>();
        CreateMap<ChecklistItem, ChecklistItemDto>();
        CreateMap<ChecklistItemDto, ChecklistItemOutput>();
        CreateMap<ListChecklistItemsInput, ListChecklistItemsRequest>();
        CreateMap<UpdateChecklistItemInput, UpdateChecklistItemRequest>();

        // v2
        CreateMap<ChecklistItemDto, v2ChecklistItemOutput>();
        CreateMap<v2ListChecklistItemsInput, ListChecklistItemsRequest>();
    }
}