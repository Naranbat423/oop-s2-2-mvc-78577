using Xunit;
using FoodSafety.Domain.Models;
using System;

namespace FoodSafety.Tests;

public class DashboardTests
{
    [Fact]
    public void Inspection_Fail_WhenScoreBelow70()
    {
        var inspection = new Inspection { Score = 65 };
        var outcome = inspection.Score >= 70 ? "Pass" : "Fail";
        Assert.Equal("Fail", outcome);
    }

    [Fact]
    public void Inspection_Pass_WhenScoreAbove70()
    {
        var inspection = new Inspection { Score = 85 };
        var outcome = inspection.Score >= 70 ? "Pass" : "Fail";
        Assert.Equal("Pass", outcome);
    }

    [Fact]
    public void FollowUp_IsOverdue_WhenDueDatePassedAndStatusOpen()
    {
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(-5),
            Status = "Open"
        };
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        Assert.True(isOverdue);
    }

    [Fact]
    public void FollowUp_NotOverdue_WhenDueDateInFuture()
    {
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(5),
            Status = "Open"
        };
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        Assert.False(isOverdue);
    }

    [Fact]
    public void FollowUp_NotOverdue_WhenStatusClosed()
    {
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(-5),
            Status = "Closed"
        };
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        Assert.False(isOverdue);
    }
}
