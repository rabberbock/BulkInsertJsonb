using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

var serviceCollection = new ServiceCollection();
serviceCollection.AddDbContext<BloggingContext>();
var services = serviceCollection.BuildServiceProvider();

var blogs = new List<Blog>()
{
    new Blog()
    {
        Url = "https://www.example.com/1",
        Posts = new List<Post>()
        {
            new Post()
            {
                Title = "Test",
                Content = "Sample Content",
                Blog = new Blog()
                {
                    Url = "https://www.example.com/2"
                },
                Comments = new List<Comment>()
                {
                    new Comment()
                    {
                        CommentId = 1000,
                        Text = "Example"
                    },
                    new Comment()
                    {
                        CommentId = 1001,
                        Text = "Example 2"
                    }
                }
            }
        }
    },
    new Blog()
    {
        Url = "https://www.example.com",
        Posts = new List<Post>
        {
            new Post()
            {
                Title = "Test",
                Content = "Sample Content",
                Blog = new Blog()
                {
                    Url = "https://www.example.com/2"
                }
            }
        }
    }
};

try
{
    var context = services.GetRequiredService<BloggingContext>();
    await context.BulkInsertAsync(blogs, opt => opt.IncludeGraph = true);
    
    // Saving regularly via EF works
    // context.AddRange(blogs);
    // await context.SaveChangesAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

public class BloggingContext : DbContext
{
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=test_db;Username=postgres;Password=password");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Blog>()
            .OwnsOne(b => b.Metadata, m =>
            {
                m.ToJson();
            });
    }
}

public class Blog
{
    public int BlogId { get; set; }
    public string Url { get; set; }

    public List<Post> Posts { get; set; }

    public BlogMetadata Metadata { get; set; } = new();
}

public class Post
{
    public int PostId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }

    public int BlogId { get; set; }
    public Blog Blog { get; set; }
    public List<Comment> Comments { get; set; }
}

public class Comment
{
    public int CommentId { get; set; }
    public string Text { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; }
}


public class BlogMetadata
{
    public bool IsPublic { get; set; }
}
