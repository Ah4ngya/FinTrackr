using FinancialTrackr.Data;
using FinancialTrackr.Data.Models;
using FinancialTrackr.Pages;
using FinancialTrackr.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class Tests
{
    #region testSession
    public class TestSession : ISession
    {
        private Dictionary<string, byte[]> _storage = new();

        public byte[]? Get(string key) => _storage.ContainsKey(key) ? _storage[key] : null;
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);

        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _storage.Keys;
        public void Clear() => _storage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
    }
    #endregion

    private User CreateTestUser(int id = 1)
    {
        return new User
        {
            Id = id,
            Username = "testuser",
            Email = "test@test.com",
            Password = "hashedpassword",
            FullName = "Test User",
            IsAdmin = 0
        };
    }
    private Expense CreateTestExpense(
    int userId = 1,
    double value = 100,
    string category = "Food",
    string payment = "Cash"
)
{
    return new Expense
    {
        UserId = userId,
        ExpenseValue = value,
        ExpenseType = category,
        PaymentMethod = payment,
        date = DateTime.Today
    };
}


    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
    private UserpageModel CreatePageModel(ApplicationDbContext context)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Session = new TestSession();
        httpContext.Session.SetInt32("UserId", 1);

        var pageContext = new PageContext
        {
            HttpContext = httpContext
        };

        var model = new UserpageModel(context, null!)
        {
            PageContext = pageContext,
            TempData = new TempDataDictionary(
                httpContext,
                Mock.Of<ITempDataProvider>()
            )
        };

        return model;
    }

    //test for adding new expenses
    [Fact]
    public void OnPost_AddsExpenseToDatabase()
    {
        var context = GetDbContext();
        context.Users.Add(CreateTestUser());
        context.SaveChanges();


        var model = CreatePageModel(context);

        model.ExpenseValue = 1000;
        model.ExpenseType = "Food";
        model.PaymentMethod = "Cash";
        model.date = DateTime.Today;

        model.OnPost();

        Assert.Single(context.Expenses);
        Assert.Equal(1000, context.Expenses.First().ExpenseValue);
    }
    // test the 80% warning
    [Fact]
    public void OnPost_ShowsWarning_WhenBudgetExceeds80Percent()
    {
        var context = GetDbContext();
        context.Users.Add(CreateTestUser());

        context.Budgets.Add(new MonthlyBudget
        {
            UserId = 1,
            Month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
            MonthlyBudgetAmount = 1000
        });

        context.Expenses.Add(CreateTestExpense(value: 800));
        context.SaveChanges();

        var model = CreatePageModel(context);
        model.ExpenseValue = 100;
        model.ExpenseType = "Food";
        model.PaymentMethod = "Cash";
        model.date = DateTime.Today;

        model.OnPost();

        Assert.NotNull(model.TempData["Warning"]);
    }

    //test for category budget exceeded
    [Fact]
    public void OnPost_ShowsWarning_WhenCategoryBudgetExceeded()
    {
        var context = GetDbContext();
        context.Users.Add(CreateTestUser());

        context.Categories.Add(new Category
        {
            UserId = 1,
            Name = "Food",
            BudgetLimit = 500
        });

        context.Expenses.Add(CreateTestExpense(
            value: 500,
            category: "Food"
        ));

        context.SaveChanges();

        var model = CreatePageModel(context);
        model.ExpenseValue = 50;
        model.ExpenseType = "Food";
        model.PaymentMethod = "Cash";
        model.date = DateTime.Today;

        model.OnPost();

        Assert.Contains("Túllépted", model.TempData["Warning"].ToString());
    }

    //test for similar category

    [Fact]
    public void OnPostAddNewCategory_WarnsIfSimilarNameExists()
    {
        var context = GetDbContext();
        context.Users.Add(CreateTestUser());
        context.SaveChanges();

        context.Categories.Add(new Category { UserId = 1, Name = "Food" });
        context.SaveChanges();

        var model = CreatePageModel(context);
        model.newCategory = "Foood";

        model.OnPostAddNewCategory();

        Assert.NotNull(model.TempData["Warning"]);
    }
    //test for xml exporting

    [Fact]
    public void OnPostExportXml_ReturnsXmlFile()
    {
        var context = GetDbContext();
        context.Users.Add(CreateTestUser());

        context.Expenses.Add(CreateTestExpense(value: 100));
        context.SaveChanges();

        var model = CreatePageModel(context);

        var result = model.OnPostExportXml() as FileResult;

        Assert.NotNull(result);
        Assert.Equal("application/xml", result.ContentType);
    }



}