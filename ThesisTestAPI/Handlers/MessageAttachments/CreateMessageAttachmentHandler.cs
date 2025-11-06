using MediatR;
using Microsoft.AspNetCore.Mvc;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.MessageAttachments;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.MessageAttachments
{
    public class CreateMessageAttachmentHandler : IRequestHandler<CreateMessageAttachmentRequest, (ProblemDetails?, List<MessageAttachmentResponse>?)>
    {
        private readonly ThesisDbContext _db;
        private readonly MessageAttachmentService _messageAttachmentService;
        public CreateMessageAttachmentHandler(MessageAttachmentService messageAttachmentService)
        {
            _messageAttachmentService = messageAttachmentService;
        }
        public async Task<(ProblemDetails?, List<MessageAttachmentResponse>?)> Handle(CreateMessageAttachmentRequest request, CancellationToken cancellationToken)
        {
            var blobFileName = await _messageAttachmentService.CreateMessageAttachments(request);
            return (null, blobFileName);
        }
    }
}
