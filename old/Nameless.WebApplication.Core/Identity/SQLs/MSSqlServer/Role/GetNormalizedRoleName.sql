SELECT
    NormalizedName
FROM Roles (NOLOCK)
WHERE
    ID = @ID;