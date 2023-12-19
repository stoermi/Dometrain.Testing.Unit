- `Create` Should return the user with a 201 status code when user was created
- `Create` Should return 400 status code when the user wasn't created
- `DeleteById` Should return 200 when the user was deleted successfully
- `DeleteById` Should return 404 when the user was not deleted

[HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteById(Guid id)
    {
        var deleted = await _userService.DeleteByIdAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
