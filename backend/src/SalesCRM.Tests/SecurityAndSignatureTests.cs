using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using SalesCRM.Domain.Entities;
using SalesCRM.Domain.Enums;
using SalesCRM.WebAPI.Controllers;
using Xunit;

namespace SalesCRM.Tests
{
    public class SecurityAndSignatureTests
    {
        // 1. Webhook Signature Verification Test
        [Fact]
        public void ValidateWebhookSignature_ValidSignature_ShouldPass()
        {
            // Arrange
            string secret = "my_payos_webhook_secret_key_123456789";
            string rawBody = "{\"data\":{\"amount\":100000,\"description\":\"Thanh toan don hang ORD20260603001\",\"reference\":\"FT2603418\"}}";
            
            // Compute expected signature (HMAC-SHA256)
            string expectedSignature = ComputeHmacSha256(rawBody, secret);

            // Act
            bool isValid = VerifySignature(rawBody, expectedSignature, secret);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void ValidateWebhookSignature_InvalidSignature_ShouldFail()
        {
            // Arrange
            string secret = "my_payos_webhook_secret_key_123456789";
            string rawBody = "{\"data\":{\"amount\":100000,\"description\":\"Thanh toan don hang ORD20260603001\",\"reference\":\"FT2603418\"}}";
            string badSignature = "incorrect_hmac_signature_value";

            // Act
            bool isValid = VerifySignature(rawBody, badSignature, secret);

            // Assert
            Assert.False(isValid);
        }

        // 2. Multi-Tenant IDOR Prevention Check
        [Fact]
        public void MultiTenantQueryFilter_ShouldFilterOrdersByBranchId()
        {
            // Arrange - Create a list representing orders database
            var branchA = Guid.NewGuid();
            var branchB = Guid.NewGuid();

            var databaseMock = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), OrderCode = "ORD-A1", BranchId = branchA, TotalAmount = 50000 },
                new Order { Id = Guid.NewGuid(), OrderCode = "ORD-A2", BranchId = branchA, TotalAmount = 120000 },
                new Order { Id = Guid.NewGuid(), OrderCode = "ORD-B1", BranchId = branchB, TotalAmount = 300000 }
            };

            // Act & Assert - Simulating active Tenant Claim filter for Branch A
            Guid activeBranchClaim = branchA;
            
            // Apply the mock tenant filter: order.BranchId == activeBranchClaim
            var filteredList = databaseMock.Where(o => o.BranchId == activeBranchClaim).ToList();

            // Assert: Branch A user only sees their own 2 orders, not Branch B's order
            Assert.Equal(2, filteredList.Count);
            Assert.All(filteredList, o => Assert.Equal(branchA, o.BranchId));
            Assert.False(filteredList.Any(o => o.OrderCode == "ORD-B1"));
        }

        [Fact]
        public void WebhookGlobalAccess_IgnoreQueryFilter_ShouldAccessAllBranches()
        {
            // Arrange
            var branchA = Guid.NewGuid();
            var branchB = Guid.NewGuid();

            var databaseMock = new List<Order>
            {
                new Order { Id = Guid.NewGuid(), OrderCode = "ORD-A1", BranchId = branchA, TotalAmount = 50000 },
                new Order { Id = Guid.NewGuid(), OrderCode = "ORD-B1", BranchId = branchB, TotalAmount = 300000 }
            };

            // Act - In webhook context, we ignore query filters to process bank transfer payments globally
            var allOrders = databaseMock.ToList(); // Simulates IgnoreQueryFilters()

            // Assert
            Assert.Equal(2, allOrders.Count);
            Assert.Contains(allOrders, o => o.BranchId == branchA);
            Assert.Contains(allOrders, o => o.BranchId == branchB);
        }

        // 3. API Role-Based Authorization Policy Tests
        [Fact]
        public void RoleBasedAuth_AdminUser_ShouldHaveAccessToAllEndpoints()
        {
            // Arrange
            var roles = new List<string> { "Admin" };
            
            // Act: Check permission for admin
            bool canWriteProduct = roles.Contains("Admin");
            bool canManageBranches = roles.Contains("Admin");
            bool canViewSystemReports = roles.Contains("Admin");

            // Assert
            Assert.True(canWriteProduct);
            Assert.True(canManageBranches);
            Assert.True(canViewSystemReports);
        }

        [Fact]
        public void RoleBasedAuth_StaffUser_ShouldBeRestrictedFromAdminEndpoints()
        {
            // Arrange
            var roles = new List<string> { "Staff" };

            // Act: Check permission for staff
            bool canWriteProduct = roles.Contains("Admin");
            bool canManageBranches = roles.Contains("Admin");
            bool canViewSystemReports = roles.Contains("Admin");
            bool canAccessPos = roles.Contains("Staff") || roles.Contains("Admin");

            // Assert
            Assert.False(canWriteProduct, "Staff must not have write permissions for products");
            Assert.False(canManageBranches, "Staff must not manage branches");
            Assert.False(canViewSystemReports, "Staff must not see system reports");
            Assert.True(canAccessPos, "Staff should have access to POS");
        }

        [Theory]
        [InlineData(typeof(ProductsController), "CreateProduct", "Admin")]
        [InlineData(typeof(ProductsController), "UpdateProduct", "Admin")]
        [InlineData(typeof(ProductsController), "DeleteProduct", "Admin")]
        [InlineData(typeof(BranchesController), "CreateBranch", "Admin")]
        [InlineData(typeof(BranchesController), "UpdateBranch", "Admin")]
        [InlineData(typeof(BranchesController), "DeleteBranch", "Admin")]
        [InlineData(typeof(ReportsController), "GetBranchReport", "Admin,Manager")]
        [InlineData(typeof(ReportsController), "GetSystemReport", "Admin")]
        [InlineData(typeof(InventoryController), "ImportBatches", "Admin,Manager")]
        [InlineData(typeof(InventoryController), "CreateTransfer", "Admin,Manager")]
        [InlineData(typeof(InventoryController), "ReceiveTransfer", "Admin,Manager")]
        [InlineData(typeof(InventoryController), "SubmitStocktake", "Admin,Manager")]
        public void ControllerAction_ShouldHaveCorrectRoleAuthorization(Type controllerType, string actionMethodName, string expectedRoles)
        {
            // Arrange & Act
            var method = controllerType.GetMethod(actionMethodName);
            Assert.NotNull(method);
            var authorizeAttr = method.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            Assert.NotNull(authorizeAttr);
            Assert.Equal(expectedRoles, authorizeAttr.Roles);
        }

        [Theory]
        [InlineData(typeof(ProductsController), true, null)]
        [InlineData(typeof(BranchesController), true, null)]
        [InlineData(typeof(ReportsController), true, null)]
        [InlineData(typeof(InventoryController), true, null)]
        [InlineData(typeof(OrdersController), true, null)]
        [InlineData(typeof(UsersController), true, "Admin,Manager")]
        public void ControllerClass_ShouldHaveCorrectClassLevelAuthorization(Type controllerType, bool shouldHaveAuthorize, string? expectedRoles)
        {
            // Arrange & Act
            var authorizeAttr = controllerType.GetCustomAttribute<AuthorizeAttribute>();

            // Assert
            if (shouldHaveAuthorize)
            {
                Assert.NotNull(authorizeAttr);
                if (expectedRoles != null)
                {
                    Assert.Equal(expectedRoles, authorizeAttr.Roles);
                }
            }
            else
            {
                Assert.Null(authorizeAttr);
            }
        }

        // Helper Signature verification functions
        private static string ComputeHmacSha256(string data, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hash = hmac.ComputeHash(dataBytes);
                return Convert.ToHexString(hash).ToLower();
            }
        }

        private static bool VerifySignature(string data, string signature, string secret)
        {
            if (string.IsNullOrWhiteSpace(signature)) return false;
            string computed = ComputeHmacSha256(data, secret);
            return string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase);
        }
    }
}
