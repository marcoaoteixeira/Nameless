SELECT
    ID,
    Name,
    NormalizedName
FROM Roles (NOLOCK)
WHERE
    NormalizedName = @NormalizedName;