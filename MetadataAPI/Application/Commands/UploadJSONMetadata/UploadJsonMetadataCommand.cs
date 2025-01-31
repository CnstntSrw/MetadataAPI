using MediatR;
using MetadataAPI.Common;

namespace MetadataAPI.Application.Commands.UploadJSONMetadata
{
    public record UploadJsonMetadataCommand(IFormFile File) : IRequest<CommonResult>;
}
