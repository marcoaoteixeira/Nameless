DELETE FROM RoleClaims
WHERE
    RoleID = @RoleID
AND Type = @Type