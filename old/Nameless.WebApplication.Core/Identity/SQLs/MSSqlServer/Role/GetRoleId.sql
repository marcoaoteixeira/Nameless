SELECT
    ID
FROM Roles (NOLOCK)
WHERE
    NormalizedName = @NormalizedName;