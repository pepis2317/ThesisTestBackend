using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.MessageAttachments;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.MessageAttachments
{
    public class CreateMessageAttachmentHandler : IRequestHandler<CreateMessageAttachmentRequest, (ProblemDetails?, string?)>
    {
        private readonly ThesisDbContext _db;
        private readonly BlobStorageService _blobStorageService;
        private readonly MessageAttachmentService _messageAttachmentService;
        public CreateMessageAttachmentHandler(MessageAttachmentService messageAttachmentService)
        {
            _messageAttachmentService = messageAttachmentService;
        }
        public async Task<(ProblemDetails?, string?)> Handle(CreateMessageAttachmentRequest request, CancellationToken cancellationToken)
        {
            var blobFileName = await _messageAttachmentService.CreateMessageAttachment(request);
            return (null, blobFileName);
        }
    }
}
