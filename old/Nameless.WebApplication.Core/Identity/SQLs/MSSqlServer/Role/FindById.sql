SELECT
    ID,
    Name,
    NormalizedName
FROM Roles (NOLOCK)
WHERE
    ID = @ID;