using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pixsper.Cueordinator.Models;

namespace Pixsper.Cueordinator.Tests;

[TestClass]
public class CueNumberMultipartTests
{
    [TestMethod]
    public void CanCompare()
    {
        var cues = new[]
        {
            new CueNumberMultipart(),
            new CueNumberMultipart(0),
            new CueNumberMultipart(0, 0),
            new CueNumberMultipart(0, 1),
            new CueNumberMultipart(1),
            new CueNumberMultipart(1, 1),
            new CueNumberMultipart(1, 1, 1),
            new CueNumberMultipart(2)
        };

        var sortedCues = cues.Order().ToList();

        sortedCues.Should().BeEquivalentTo(cues, options => options.WithStrictOrdering());
    }
}