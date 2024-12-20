using FluentAssertions;
using JsonApiClient.Attributes;
using JsonApiClient.Models;
using JsonApiClient.Statements;
using JsonApiClient.Tests.Models;

namespace JsonApiClient.Tests.Statements;

public class WhereStatementTests
{
    private class TestModel : JResource<int>
    {
        [JAttr]
        public string? LastName { get; set; }
        [JAttr]
        public int Age { get; set; }
        [JAttr]
        public string? Description { get; set; }
        [JAttr]
        public string? Chapter { get; set; }
        [JAttr]
        public DateTime LastModified { get; set; }
        [JAttr]
        public TimeSpan Duration { get; set; }
        [JAttr]
        public double Percentage { get; set; }
        [JAttr]
        public bool Active { get; set; }
        [JRel]
        public ICollection<Author> Authors { get; set; }
    }

    [Fact]
    public void Parse_EqualityCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.LastName == "Smith").Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("equals(lastName,'Smith')");
    }
    
    [Fact]
    public void Parse_PropertyNegation_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => !m.Active).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("not(equals(active,'true'))");
    }
    
    [Fact]
    public void Parse_PropertyBoolCheck_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Active).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("equals(active,'true')");
    }

    [Fact]
    public void Parse_InequalityCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.LastName != "Smith").Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("not(equals(lastName,'Smith'))");
    }

    [Fact]
    public void Parse_GreaterThanCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Age > 25).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("greaterThan(age,25)");
    }

    [Fact]
    public void Parse_LessThanOrEqualCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Age <= 30).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("lessOrEqual(age,30)");
    }

    [Fact]
    public void Parse_StringContainsCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Description.Contains("cooking")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("contains(description,'cooking')");
    }

    [Fact]
    public void Parse_StringStartsWithCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Description.StartsWith("The")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("startsWith(description,'The')");
    }

    [Fact]
    public void Parse_MultipleConditionsWithAnd_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => 
            m.LastName == "Smith" && m.Age > 25 && m.Description!.Contains("cooking")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("and(and(equals(lastName,'Smith'),greaterThan(age,25)),contains(description,'cooking'))");
    }

    [Fact]
    public void Parse_MultipleConditionsWithOr_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => 
            m.LastName == "Smith" || m.Age > 25 || m.Description!.Contains("cooking")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("or(or(equals(lastName,'Smith'),greaterThan(age,25)),contains(description,'cooking'))");
    }

    [Fact]
    public void Parse_DateTimeComparison_ShouldReturnCorrectFilter()
    {
        var date = new DateTime(2001, 1, 1, 0, 0, 0);
        var result = new WhereStatement<TestModel,TestModel>(m => m.LastModified <= date).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("lessOrEqual(lastModified,'2001-01-01 00:00:00')");
    }

    [Fact]
    public void Parse_TimeSpanComparison_ShouldReturnCorrectFilter()
    {
        var duration = new TimeSpan(6, 12, 14);
        var result = new WhereStatement<TestModel,TestModel>(m => m.Duration > duration).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("greaterThan(duration,'06:12:14')");
    }

    [Fact]
    public void Parse_DoubleComparison_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Percentage >= 33.33).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("greaterOrEqual(percentage,33.33)");
    }

    [Fact]
    public void Parse_AnyCondition_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => 
            new[] { "Intro", "Summary", "Conclusion" }.Contains(m.Chapter)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("any(chapter,'Intro','Summary','Conclusion')");
    }

    [Fact]
    public void Parse_AnyConditionWithList_ShouldReturnCorrectFilter()
    {
        var chapters = new List<string> { "Intro", "Summary", "Conclusion" };
        var result = new WhereStatement<TestModel,TestModel>(m => chapters.Contains(m.Chapter)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("any(chapter,'Intro','Summary','Conclusion')");
    }
    [Fact]
    public void Parse_AnyConditionWithArray_ShouldReturnCorrectFilter()
    {
        var chapters = new[] { "Intro", "Summary", "Conclusion" };
        var result = new WhereStatement<TestModel,TestModel>(m => chapters.Contains(m.Chapter)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("any(chapter,'Intro','Summary','Conclusion')");
    }

    [Fact]
    public void Parse_AnyConditionWithInlineCollection_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => 
            new List<string> { "Intro", "Summary", "Conclusion" }.Contains(m.Chapter!)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("any(chapter,'Intro','Summary','Conclusion')");
    }

    [Fact]
    public void Parse_AnyConditionWithInlineArray_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => 
            new[] { "Intro", "Summary", "Conclusion" }.Contains(m.Chapter)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("any(chapter,'Intro','Summary','Conclusion')");
    }
    
    [Fact]
    public void Parse_ComplexLambda1_ShouldReturnCorrectFilter()
    {
        var dateTime = DateTime.Parse("2022/05/12");
        var result = new WhereStatement<TestModel,TestModel>(m => 
            new[] { "Intro", "Summary", "Conclusion" }.Contains(m.Chapter) && !m.Description!.Contains("ciao") || m.LastModified <= dateTime).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("or(and(any(chapter,'Intro','Summary','Conclusion'),not(contains(description,'ciao'))),lessOrEqual(lastModified,'2022-05-12 00:00:00'))");
    }
    
    [Fact]
    public void Parse_ComplexLambda2_ShouldReturnCorrectFilter()
    {
        var dateTime = DateTime.Parse("2022/05/12");
        var result = new WhereStatement<TestModel,TestModel>(m => 
            new[] { "Intro", "Summary", "Conclusion" }.Contains(m.Chapter) && (!m.Description!.Contains("ciao") || m.LastModified <= dateTime)).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("and(any(chapter,'Intro','Summary','Conclusion'),or(not(contains(description,'ciao')),lessOrEqual(lastModified,'2022-05-12 00:00:00')))");
    }
    
    [Fact]
    public void Parse_StringEndsWith_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Description!.EndsWith("ciao")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("endsWith(description,'ciao')");
    }
    
    [Fact]
    public void Parse_StringStartsWith_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Description!.StartsWith("ciao")).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("startsWith(description,'ciao')");
    }
    
    [Fact]
    public void Parse_HasRelatedResources_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Authors.Any()).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("has(authors)");
    }
    
    [Fact]
    public void Parse_ConditionOnSubresource_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Authors.Any(a => a.FirstName == "Tesla") ).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("equals(authors.firstName,'Tesla')");
    }
    
    [Fact]
    public void Parse_BooleanConditionOnSubresource_ShouldReturnCorrectFilter()
    {
        var result = new WhereStatement<TestModel,TestModel>(m => m.Authors.Any(a => a.Books.Any(b => !b.Deleted)) ).Translate();

        result.Key.Should().Be("filter");
        result.Value.Should().Be("not(equals(authors.books.deleted,'true'))");
    }
}