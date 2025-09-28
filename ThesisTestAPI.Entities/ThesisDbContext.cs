using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ThesisTestAPI.Entities;

public partial class ThesisDbContext : DbContext
{
    public ThesisDbContext()
    {
    }

    public ThesisDbContext(DbContextOptions<ThesisDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<Conversation> Conversations { get; set; }

    public virtual DbSet<ConversationMember> ConversationMembers { get; set; }

    public virtual DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Process> Processes { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Reaction> Reactions { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<SellerReview> SellerReviews { get; set; }

    public virtual DbSet<Step> Steps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserReview> UserReviews { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<WalletTransaction> WalletTransactions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Server=tcp:thesis-db.database.windows.net,1433;Initial Catalog=ThesisDB;Persist Security Info=False;User ID=admin_user;Password=password11!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA137AB71B");

            entity.Property(e => e.CommentId).ValueGeneratedNever();
            entity.Property(e => e.Comment1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Comment");

            entity.HasOne(d => d.CommentNavigation).WithOne(p => p.CommentCommentNavigation)
                .HasForeignKey<Comment>(d => d.CommentId)
                .HasConstraintName("FK__Comments__Commen__6166761E");

            entity.HasOne(d => d.TargetContent).WithMany(p => p.CommentTargetContents)
                .HasForeignKey(d => d.TargetContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Comments__Target__625A9A57");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PK__Content__2907A81E7BC077FD");

            entity.ToTable("Content");

            entity.Property(e => e.ContentId).ValueGeneratedNever();

            entity.HasOne(d => d.Author).WithMany(p => p.Contents)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Content__AuthorI__3F115E1A");
        });

        modelBuilder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.ConversationId).HasName("PK__Conversa__C050D8774D78D97F");

            entity.Property(e => e.ConversationId).ValueGeneratedNever();
            entity.Property(e => e.ConversationName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ConversationMember>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Conversa__0CF04B18A319C0DE");

            entity.Property(e => e.MemberId).ValueGeneratedNever();

            entity.HasOne(d => d.Conversation).WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversat__Conve__2BC97F7C");

            entity.HasOne(d => d.User).WithMany(p => p.ConversationMembers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Conversat__UserI__2CBDA3B5");
        });

        modelBuilder.Entity<DataProtectionKey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DataProt__3214EC0711B7C954");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70CF5A35C57");

            entity.Property(e => e.ImageId).ValueGeneratedNever();
            entity.Property(e => e.ImageName)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Content).WithMany(p => p.Images)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("FK__Images__ContentI__65370702");
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__Likes__A2922C14FD9B27CF");

            entity.Property(e => e.LikeId).ValueGeneratedNever();

            entity.HasOne(d => d.LikeNavigation).WithOne(p => p.Like)
                .HasForeignKey<Like>(d => d.LikeId)
                .HasConstraintName("FK__Likes__LikeId__4C6B5938");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__C87C0C9CBDF3591C");

            entity.Property(e => e.MessageId).ValueGeneratedNever();
            entity.Property(e => e.Message1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("Message");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Conversation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ConversationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Conver__2704CA5F");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__27F8EE98");
        });

        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.HasKey(e => e.AttachmentId).HasName("PK__MessageA__442C64BEFB941AC4");

            entity.HasIndex(e => e.BlobFileName, "UQ__MessageA__459CB19B3E9D6F13").IsUnique();

            entity.Property(e => e.AttachmentId).ValueGeneratedNever();
            entity.Property(e => e.BlobFileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FileType)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Message).WithMany(p => p.MessageAttachments)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__MessageAt__Messa__2F9A1060");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA1260182C4741E2");

            entity.Property(e => e.PostId).ValueGeneratedNever();
            entity.Property(e => e.Caption)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.PostNavigation).WithOne(p => p.Post)
                .HasForeignKey<Post>(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Posts__PostId__503BEA1C");
        });

        modelBuilder.Entity<Process>(entity =>
        {
            entity.HasKey(e => e.ProcessId).HasName("PK__Processe__1B39A9561F2053BA");

            entity.Property(e => e.ProcessId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Request).WithMany(p => p.Processes)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK__Processes__Reque__74794A92");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.RatingId).HasName("PK__Ratings__FCCDF87C3FB1462B");

            entity.Property(e => e.RatingId).ValueGeneratedNever();
            entity.Property(e => e.Rating1).HasColumnName("Rating");

            entity.HasOne(d => d.RatingNavigation).WithOne(p => p.Rating)
                .HasForeignKey<Rating>(d => d.RatingId)
                .HasConstraintName("FK__Ratings__RatingI__498EEC8D");
        });

        modelBuilder.Entity<Reaction>(entity =>
        {
            entity.HasKey(e => e.ReactionId).HasName("PK__Reaction__46DDF9B4DD61D4CE");

            entity.Property(e => e.ReactionId).ValueGeneratedNever();

            entity.HasOne(d => d.Author).WithMany(p => p.Reactions)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reactions__Autho__42E1EEFE");

            entity.HasOne(d => d.Content).WithMany(p => p.Reactions)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("FK_Reactions_Content");
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__Requests__33A8517A88319411");

            entity.Property(e => e.RequestId).ValueGeneratedNever();
            entity.Property(e => e.RequestMessage)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RequestStatus)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RequestTitle)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.RequestNavigation).WithOne(p => p.Request)
                .HasForeignKey<Request>(d => d.RequestId)
                .HasConstraintName("FK__Requests__Reques__6FB49575");

            entity.HasOne(d => d.Seller).WithMany(p => p.Requests)
                .HasForeignKey(d => d.SellerId)
                .HasConstraintName("FK__Requests__Produc__70A8B9AE");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__Producer__13369652E029C705");

            entity.HasIndex(e => e.SellerName, "UQ__Producer__E0E723B8321AB5E4").IsUnique();

            entity.Property(e => e.SellerId).ValueGeneratedNever();
            entity.Property(e => e.Banner)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.SellerName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.SellerPicture)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Owner).WithMany(p => p.Sellers)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Producers__Owner__07C12930");
        });

        modelBuilder.Entity<SellerReview>(entity =>
        {
            entity.HasKey(e => e.SellerReviewId).HasName("PK__Producer__8AA2C4138B0CED33");

            entity.Property(e => e.SellerReviewId).ValueGeneratedNever();
            entity.Property(e => e.Review)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Seller).WithMany(p => p.SellerReviews)
                .HasForeignKey(d => d.SellerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProducerR__Produ__6CD828CA");

            entity.HasOne(d => d.SellerReviewNavigation).WithOne(p => p.SellerReview)
                .HasForeignKey<SellerReview>(d => d.SellerReviewId)
                .HasConstraintName("FK__ProducerR__Produ__6BE40491");
        });

        modelBuilder.Entity<Step>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PK__Steps__24343357128540E3");

            entity.Property(e => e.StepId).ValueGeneratedNever();
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.NextStep).WithMany(p => p.InverseNextStep)
                .HasForeignKey(d => d.NextStepId)
                .HasConstraintName("FK__Steps__NextStepI__1E6F845E");

            entity.HasOne(d => d.Process).WithMany(p => p.Steps)
                .HasForeignKey(d => d.ProcessId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Steps__ProcessId__1C873BEC");

            entity.HasOne(d => d.StepNavigation).WithOne(p => p.Step)
                .HasForeignKey<Step>(d => d.StepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Steps__StepId__1B9317B3");

            entity.HasOne(d => d.Transaction).WithMany(p => p.Steps)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK__Steps__Transacti__1D7B6025");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CDCC33ED4");

            entity.HasIndex(e => e.Phone, "UQ__Users__5C7E359E3F085A10").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DB10C8C3").IsUnique();

            entity.HasIndex(e => e.UserName, "UQ__Users__C9F284564A8A4CFC").IsUnique();

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Pfp).HasMaxLength(500);
            entity.Property(e => e.Phone)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.RefreshTokenExpiryTime).HasColumnType("datetime");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("User");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserReview>(entity =>
        {
            entity.HasKey(e => e.UserReviewId).HasName("PK__UserRevi__238D9AA39D8362D1");

            entity.Property(e => e.UserReviewId).ValueGeneratedNever();
            entity.Property(e => e.Review)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.UserReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRevie__UserI__690797E6");

            entity.HasOne(d => d.UserReviewNavigation).WithOne(p => p.UserReview)
                .HasForeignKey<UserReview>(d => d.UserReviewId)
                .HasConstraintName("FK__UserRevie__UserR__681373AD");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasIndex(e => new { e.UserId, e.Currency }, "UQ_Wallets_User_Currency").IsUnique();

            entity.Property(e => e.WalletId).ValueGeneratedNever();
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("active");

            entity.HasOne(d => d.User).WithMany(p => p.Wallets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Wallets_Users");
        });

        modelBuilder.Entity<WalletTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId);

            entity.HasIndex(e => e.TransferGroupId, "IX_WT_TransferGroup");

            entity.HasIndex(e => new { e.WalletId, e.Status, e.PostedAt }, "IX_WT_Wallet_Status_PostedAt");

            entity.HasIndex(e => new { e.WalletId, e.ExternalRef }, "UQ_WT_Wallet_ExternalRef")
                .IsUnique()
                .HasFilter("([ExternalRef] IS NOT NULL)");

            entity.HasIndex(e => new { e.WalletId, e.IdempotencyKey }, "UQ_WT_Wallet_Idempotency")
                .IsUnique()
                .HasFilter("([IdempotencyKey] IS NOT NULL)");

            entity.Property(e => e.TransactionId).ValueGeneratedNever();
            entity.Property(e => e.Direction)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ExternalRef)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.IdempotencyKey)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Memo).HasMaxLength(200);
            entity.Property(e => e.PostedAt).HasColumnType("datetime");
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.SignedAmount).HasComputedColumnSql("(case when [Direction]='C' then [AmountMinor] else  -[AmountMinor] end)", true);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("posted");
            entity.Property(e => e.Type)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.VoidedAt).HasColumnType("datetime");

            entity.HasOne(d => d.ParentTransaction).WithMany(p => p.InverseParentTransaction)
                .HasForeignKey(d => d.ParentTransactionId)
                .HasConstraintName("FK_WT_Parent");

            entity.HasOne(d => d.Wallet).WithMany(p => p.WalletTransactions)
                .HasForeignKey(d => d.WalletId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WalletTransactions_Wallets");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
