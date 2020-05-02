UPDATE Roles SET
    Name = @Name,
    NormalizedName = @NormalizedName
WHERE
    ID = @ID;