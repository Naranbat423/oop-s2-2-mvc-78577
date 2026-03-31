using Xunit;
using FoodSafety.Domain.Models;
using System;

namespace FoodSafety.Tests;

public class DashboardTests
{
    [Fact]
    public void Inspection_Fail_WhenScoreBelow70()
    {
        // Arrange
        var inspection = new Inspection { Score = 65 };
        
        // Act
        var outcome = inspection.Score >= 70 ? "Pass" : "Fail";
        
        // Assert
        Assert.Equal("Fail", outcome);
    }

    [Fact]
    public void Inspection_Pass_WhenScoreAbove70()
    {
        // Arrange
        var inspection = new Inspection { Score = 85 };
        
        // Act
        var outcome = inspection.Score >= 70 ? "Pass" : "Fail";
        
        // Assert
        Assert.Equal("Pass", outcome);
    }

    [Fact]
    public void FollowUp_IsOverdue_WhenDueDatePassedAndStatusOpen()
    {
        // Arrange
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(-5),
            Status = "Open"
        };
        
        // Act
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        
        // Assert
        Assert.True(isOverdue);
    }

    [Fact]
    public void FollowUp_NotOverdue_WhenDueDateInFuture()
    {
        // Arrange
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(5),
            Status = "Open"
        };
        
        // Act
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        
        // Assert
        Assert.False(isOverdue);
    }

    [Fact]
    public void FollowUp_NotOverdue_WhenStatusClosed()
    {
        // Arrange
        var followUp = new FollowUp 
        { 
            DueDate = DateTime.Today.AddDays(-5),
            Status = "Closed"
        };
        
        // Act
        var isOverdue = followUp.Status == "Open" && followUp.DueDate < DateTime.Today;
        
        // Assert
        Assert.False(isOverdue);
    }
}
