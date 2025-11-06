using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using ThesisTestAPI.Entities;
using ThesisTestAPI.Models.Chat;
using ThesisTestAPI.Services;

namespace ThesisTestAPI.Handlers.Chat
{
    public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, (ProblemDetails?, CursorPage<MessageResponse>?)>
    {
        private readonly ThesisDbContext _db;
        public GetMessagesHandler(ThesisDbContext db)
        {
            _db = db;
        }

        public async Task<(ProblemDetails?, CursorPage<MessageResponse>?)> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var limit = Math.Clamp(request.Limit <= 0 ? 50 : request.Limit, 1, 200);
            var q = _db.Messages.AsNoTracking()
                .Where(m => m.ConversationId == request.ConversationId);

            IOrderedQueryable<Message> ordered;
            bool backward = !string.IsNullOrEmpty(request.Before);
            bool isInitialLatest = string.IsNullOrEmpty(request.Before) && string.IsNullOrEmpty(request.After);

            // Choose ordering
            if (backward)
            {
                var (cAt, cId) = Cursor.Decode(request.Before!);
                q = q.Where(m =>
                    (m.CreatedAt < cAt) ||
                    (m.CreatedAt == cAt && m.MessageId.CompareTo(cId) < 0));
                ordered = q.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.MessageId);
            }
            else if (!string.IsNullOrEmpty(request.After))
            {
                var (cAt, cId) = Cursor.Decode(request.After!);
                q = q.Where(m =>
                    (m.CreatedAt > cAt) ||
                    (m.CreatedAt == cAt && m.MessageId.CompareTo(cId) > 0));
                ordered = q.OrderBy(m => m.CreatedAt).ThenBy(m => m.MessageId);
            }
            else
            {
                // initial: newest page
                ordered = q.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.MessageId);
            }

            bool queriedDesc = backward || isInitialLatest;

            // 1) Fetch in DB order
            var pageRaw = await ordered
                .Take(limit + 1) // lookahead
                .ToListAsync(cancellationToken);

            // 2) Trim the lookahead ON THE DB-ORDER SIDE
            bool hasMore = pageRaw.Count > limit;
            if (hasMore)
            {
                // In DB order, the lookahead is ALWAYS the last element
                pageRaw.RemoveAt(pageRaw.Count - 1);
            }

            // 3) Normalize to ASC for the client exactly once
            List<Message> rows = queriedDesc
                ? pageRaw.AsEnumerable().Reverse().ToList() // DESC -> ASC
                : pageRaw;                                   // already ASC

            // 4) Map
            var items = rows.Select(m => new MessageResponse
            {
                MessageId = m.MessageId,
                SenderId = m.SenderId,
                Message = m.Message1,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt,
                DeletedAt = m.DeletedAt,
                HasAttachments = m.HasAttachments
            }).ToList();

            // 5) Cursors (rows are ASC here)
            string? next = null;
            string? prev = null;

            if (items.Count > 0)
            {
                var first = rows.First(); // oldest in slice
                var last = rows.Last();  // newest in slice

                if (isInitialLatest)
                {
                    // at tail: nothing newer
                    next = null;
                    // older exists if hasMore (we trimmed because there’s more older data)
                    prev = hasMore ? Cursor.Encode(first.CreatedAt.ToUniversalTime(), first.MessageId) : null;
                }
                else
                {
                    // normal pages
                    next = hasMore
                        ? Cursor.Encode(last.CreatedAt.ToUniversalTime(), last.MessageId)   // fetch newer than 'last'
                        : null;

                    prev = Cursor.Encode(first.CreatedAt.ToUniversalTime(), first.MessageId); // fetch older than 'first'
                }
            }

            var page = new CursorPage<MessageResponse>
            {
                Items = items,
                NextCursor = next,
                PrevCursor = prev,
                HasNext = isInitialLatest ? false : hasMore,                 // initial latest: no newer
                HasPrev = isInitialLatest ? hasMore : items.Count > 0        // initial latest: older if hasMore
            };


            return (null, page);
        }
    }
}
