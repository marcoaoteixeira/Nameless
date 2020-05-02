SELECT
    RoleID,
    Type,
    Value
FROM RoleClaims (NOLOCK)
WHERE
    RoleID = @RoleID