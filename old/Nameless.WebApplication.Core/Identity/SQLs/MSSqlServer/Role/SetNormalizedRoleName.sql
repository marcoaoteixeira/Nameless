UPDATE Roles SET
    NormalizedName = @NormalizedName
WHERE
    ID = @ID;