using Microsoft.AspNetCore.Mvc;
using PosSystem.Attributes;
using PosSystem.Services;

namespace PosSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UserManagementController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                var userDtos = users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.PhoneNumber,
                    u.IsActive,
                    u.StoreId,
                    StoreName = u.Store?.StoreName,
                    u.CreatedAt,
                    u.UpdatedAt,
                    u.LastLoginAt
                });

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.PhoneNumber,
                    user.PIN,
                    user.IsActive,
                    user.StoreId,
                    StoreName = user.Store?.StoreName,
                    user.CreatedAt,
                    user.UpdatedAt,
                    user.LastLoginAt
                };

                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving user", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new user (Admin only)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManagementService.CreateUserAsync(request);
                
                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.PhoneNumber,
                    user.IsActive,
                    user.StoreId,
                    user.CreatedAt
                };

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, userDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManagementService.UpdateUserAsync(id, request);
                
                var userDto = new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Role,
                    user.PhoneNumber,
                    user.IsActive,
                    user.StoreId,
                    user.UpdatedAt
                };

                return Ok(userDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await _userManagementService.DeleteUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting user", error = ex.Message });
            }
        }

        /// <summary>
        /// Activate user (Admin only)
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userManagementService.ActivateUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while activating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate user (Admin only)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userManagementService.DeactivateUserAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deactivating user", error = ex.Message });
            }
        }

        /// <summary>
        /// Reset user password (Admin only)
        /// </summary>
        [HttpPost("{id}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetNewPasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userManagementService.ResetPasswordAsync(id, request.NewPassword);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resetting password", error = ex.Message });
            }
        }

        /// <summary>
        /// Change user PIN (Admin only)
        /// </summary>
        [HttpPost("{id}/change-pin")]
        public async Task<IActionResult> ChangePIN(int id, [FromBody] ChangePINRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _userManagementService.ChangePINAsync(id, request.NewPIN);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "PIN changed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while changing PIN", error = ex.Message });
            }
        }

        /// <summary>
        /// Get users by role
        /// </summary>
        [HttpGet("by-role/{role}")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                var users = await _userManagementService.GetUsersByRoleAsync(role);
                var userDtos = users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.IsActive,
                    u.StoreId,
                    StoreName = u.Store?.StoreName
                });

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users by role", error = ex.Message });
            }
        }

        /// <summary>
        /// Get users by store
        /// </summary>
        [HttpGet("by-store/{storeId}")]
        public async Task<IActionResult> GetUsersByStore(int storeId)
        {
            try
            {
                var users = await _userManagementService.GetUsersByStoreAsync(storeId);
                var userDtos = users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.Role,
                    u.IsActive,
                    u.StoreId,
                    StoreName = u.Store?.StoreName
                });

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving users by store", error = ex.Message });
            }
        }

        /// <summary>
        /// Assign user to store (Admin only)
        /// </summary>
        [HttpPost("{userId}/assign-store/{storeId}")]
        public async Task<IActionResult> AssignUserToStore(int userId, int storeId)
        {
            try
            {
                var result = await _userManagementService.AssignUserToStoreAsync(userId, storeId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User assigned to store successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while assigning user to store", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove user from store (Admin only)
        /// </summary>
        [HttpPost("{userId}/remove-store")]
        public async Task<IActionResult> RemoveUserFromStore(int userId)
        {
            try
            {
                var result = await _userManagementService.RemoveUserFromStoreAsync(userId);
                if (!result)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(new { message = "User removed from store successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while removing user from store", error = ex.Message });
            }
        }
    }

    public class ResetNewPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePINRequest
    {
        public string NewPIN { get; set; } = string.Empty;
    }
}