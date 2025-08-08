using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosSystem.Data;
using PosSystem.Models;

namespace PosSystem.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public UserManagementService(
            UserManager<User> userManager,
            ApplicationDbContext context,
            IAuditService auditService)
        {
            _userManager = userManager;
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Store)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Store)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            // Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Check if email already exists
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Validate store exists if provided
            if (request.StoreId.HasValue)
            {
                var storeExists = await _context.Stores.AnyAsync(s => s.Id == request.StoreId.Value);
                if (!storeExists)
                {
                    throw new InvalidOperationException("Store not found");
                }
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = request.Role,
                PIN = request.PIN,
                StoreId = request.StoreId,
                PhoneNumber = request.PhoneNumber,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user: {errors}");
            }

            // Log the creation
            await _auditService.LogActionAsync(user.Id, "CREATE", "User", user.Id, null, 
                new { UserName = user.UserName, Role = user.Role });

            return user;
        }

        public async Task<User> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new InvalidOperationException("User not found");
            }

            var changes = new List<string>();

            // Update username if provided and different
            if (!string.IsNullOrEmpty(request.UserName) && request.UserName != user.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(request.UserName);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    throw new InvalidOperationException("Username already exists");
                }
                changes.Add($"Username: {user.UserName} -> {request.UserName}");
                user.UserName = request.UserName;
            }

            // Update email if provided and different
            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                var existingEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingEmail != null && existingEmail.Id != user.Id)
                {
                    throw new InvalidOperationException("Email already exists");
                }
                changes.Add($"Email: {user.Email} -> {request.Email}");
                user.Email = request.Email;
            }

            // Update other fields
            if (!string.IsNullOrEmpty(request.FirstName) && request.FirstName != user.FirstName)
            {
                changes.Add($"FirstName: {user.FirstName} -> {request.FirstName}");
                user.FirstName = request.FirstName;
            }

            if (!string.IsNullOrEmpty(request.LastName) && request.LastName != user.LastName)
            {
                changes.Add($"LastName: {user.LastName} -> {request.LastName}");
                user.LastName = request.LastName;
            }

            if (!string.IsNullOrEmpty(request.Role) && request.Role != user.Role)
            {
                changes.Add($"Role: {user.Role} -> {request.Role}");
                user.Role = request.Role;
            }

            if (request.PIN != null && request.PIN != user.PIN)
            {
                changes.Add("PIN updated");
                user.PIN = request.PIN;
            }

            if (request.StoreId != user.StoreId)
            {
                if (request.StoreId.HasValue)
                {
                    var storeExists = await _context.Stores.AnyAsync(s => s.Id == request.StoreId.Value);
                    if (!storeExists)
                    {
                        throw new InvalidOperationException("Store not found");
                    }
                }
                changes.Add($"StoreId: {user.StoreId} -> {request.StoreId}");
                user.StoreId = request.StoreId;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber) && request.PhoneNumber != user.PhoneNumber)
            {
                changes.Add($"PhoneNumber: {user.PhoneNumber} -> {request.PhoneNumber}");
                user.PhoneNumber = request.PhoneNumber;
            }

            if (request.IsActive.HasValue && request.IsActive.Value != user.IsActive)
            {
                changes.Add($"IsActive: {user.IsActive} -> {request.IsActive.Value}");
                user.IsActive = request.IsActive.Value;
            }

            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user: {errors}");
            }

            // Log the changes
            if (changes.Any())
            {
                await _auditService.LogActionAsync(user.Id, "UPDATE", "User", user.Id, null, 
                    new { Changes = string.Join(", ", changes) });
            }

            return user;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            // Check if user has any transactions
            var hasTransactions = await _context.Transactions.AnyAsync(t => t.UserId == id);
            if (hasTransactions)
            {
                throw new InvalidOperationException("Cannot delete user with existing transactions. Deactivate instead.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(id, "DELETE", "User", id, null, 
                    new { UserName = user.UserName });
                return true;
            }

            return false;
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            if (user.IsActive)
            {
                return true; // Already active
            }

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(id, "ACTIVATE", "User", id, null, 
                    new { UserName = user.UserName });
                return true;
            }

            return false;
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            if (!user.IsActive)
            {
                return true; // Already inactive
            }

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(id, "DEACTIVATE", "User", id, null, 
                    new { UserName = user.UserName });
                return true;
            }

            return false;
        }

        public async Task<bool> ResetPasswordAsync(int id, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (result.Succeeded)
            {
                user.UpdatedAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                await _auditService.LogActionAsync(id, "PASSWORD_RESET", "User", id, null, 
                    new { UserName = user.UserName });
                return true;
            }

            return false;
        }

        public async Task<bool> ChangePINAsync(int id, string newPIN)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }

            user.PIN = newPIN;
            user.UpdatedAt = DateTime.UtcNow;
            
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(id, "PIN_CHANGE", "User", id, null, 
                    new { UserName = user.UserName });
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Include(u => u.Store)
                .Where(u => u.Role == role)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByStoreAsync(int storeId)
        {
            return await _context.Users
                .Include(u => u.Store)
                .Where(u => u.StoreId == storeId)
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<bool> AssignUserToStoreAsync(int userId, int storeId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            var storeExists = await _context.Stores.AnyAsync(s => s.Id == storeId);
            if (!storeExists)
            {
                throw new InvalidOperationException("Store not found");
            }

            user.StoreId = storeId;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(userId, "STORE_ASSIGN", "User", userId, null, 
                    new { UserName = user.UserName, StoreId = storeId });
                return true;
            }

            return false;
        }

        public async Task<bool> RemoveUserFromStoreAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            var oldStoreId = user.StoreId;
            user.StoreId = null;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(userId, "STORE_REMOVE", "User", userId, null, 
                    new { UserName = user.UserName, OldStoreId = oldStoreId });
                return true;
            }

            return false;
        }
    }
}